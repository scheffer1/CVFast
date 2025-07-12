using CVFastApi.Integration.DTOs;
using CVFastServices.DTOs;
using CVFastServices.Models;
using CVFastServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CVFastApi.Integration.Controllers
{
    /// <summary>
    /// Controller para consulta pública de currículos por sistemas integradores
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CurriculumController : ControllerBase
    {
        private readonly IShortLinkService _shortLinkService;
        private readonly ILogger<CurriculumController> _logger;

        public CurriculumController(
            IShortLinkService shortLinkService,
            ILogger<CurriculumController> logger)
        {
            _shortLinkService = shortLinkService;
            _logger = logger;
        }

        /// <summary>
        /// Consulta um currículo através de um link curto
        /// </summary>
        /// <param name="hash">Hash do link curto do currículo</param>
        /// <returns>Dados completos do currículo</returns>
        /// <response code="200">Retorna os dados do currículo</response>
        /// <response code="404">Currículo não encontrado, link inválido ou revogado</response>
        /// <response code="403">Currículo privado - acesso negado</response>
        [HttpGet("{hash}")]
        [ProducesResponseType(typeof(ApiResponse<CurriculumIntegrationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByHash(string hash)
        {
            try
            {
                var curriculum = await _shortLinkService.GetCurriculumByHashAsync(hash);
                if (curriculum == null)
                {
                    _logger.LogWarning("Tentativa de acesso a currículo com hash inválido: {Hash}", hash);
                    return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
                }

                // Verificar se o currículo é público
                if (curriculum.Status != CurriculumStatus.Active)
                {
                    _logger.LogWarning("Tentativa de acesso a currículo privado via hash: {Hash}", hash);
                    return StatusCode(403, ApiResponse<object>.ErrorResponse("Acesso negado - currículo privado"));
                }

                // Encontrar o ShortLink para registrar o acesso
                var shortLink = curriculum.ShortLinks.FirstOrDefault(s => s.Hash == hash && !s.IsRevoked);
                if (shortLink != null)
                {
                    // Registrar o acesso
                    await _shortLinkService.RegisterAccessAsync(
                        shortLink.Id,
                        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                        HttpContext.Request.Headers.UserAgent.ToString());
                }

                _logger.LogInformation("Acesso ao currículo via API de integração - Hash: {Hash}", hash);

                return Ok(ApiResponse<CurriculumIntegrationDTO>.SuccessResponse(MapToIntegrationDto(curriculum)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar currículo com hash: {Hash}", hash);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno do servidor"));
            }
        }

        private static CurriculumIntegrationDTO MapToIntegrationDto(Curriculum curriculum)
        {
            return new CurriculumIntegrationDTO
            {
                Title = curriculum.Title,
                Summary = curriculum.Summary,
                Experiences = curriculum.Experiences?.Select(e => new ExperienceIntegrationDTO
                {
                    CompanyName = e.CompanyName,
                    Role = e.Role,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Location = e.Location
                }).ToList() ?? new List<ExperienceIntegrationDTO>(),
                Educations = curriculum.Educations?.Select(e => new EducationIntegrationDTO
                {
                    Institution = e.Institution,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Description = e.Description
                }).ToList() ?? new List<EducationIntegrationDTO>(),
                Skills = curriculum.Skills?.Select(s => new SkillIntegrationDTO
                {
                    TechName = s.TechName,
                    ProficiencyLevel = s.Proficiency
                }).ToList() ?? new List<SkillIntegrationDTO>(),
                Languages = curriculum.Languages?.Select(l => new LanguageIntegrationDTO
                {
                    LanguageName = l.LanguageName,
                    Proficiency = l.Proficiency
                }).ToList() ?? new List<LanguageIntegrationDTO>(),
                Contacts = curriculum.Contacts?.Select(c => new ContactIntegrationDTO
                {
                    Type = c.Type,
                    Value = c.Value,
                    IsPrimary = c.IsPrimary
                }).ToList() ?? new List<ContactIntegrationDTO>(),
                Address = curriculum.Addresses?.FirstOrDefault() != null ? curriculum.Addresses.FirstOrDefault()?.Street : ""
            };
        }
    }
}
