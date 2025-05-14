using CVFastApi.DTOs;
using CVFastApi.Models;
using CVFastApi.Repositories.Interfaces;
using CVFastApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVFastApi.Controllers
{
    /// <summary>
    /// Controller para operações relacionadas a currículos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class CurriculumsController : ControllerBase
    {
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly IUserRepository _userRepository;
        private readonly IShortLinkService _shortLinkService;
        private readonly IRepository<AccessLog> _accessLogRepository;
        private readonly ILogger<CurriculumsController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="curriculumRepository">Repositório de currículos</param>
        /// <param name="userRepository">Repositório de usuários</param>
        /// <param name="shortLinkService">Serviço de links curtos</param>
        /// <param name="accessLogRepository">Repositório de logs de acesso</param>
        /// <param name="logger">Logger</param>
        public CurriculumsController(
            ICurriculumRepository curriculumRepository,
            IUserRepository userRepository,
            IShortLinkService shortLinkService,
            IRepository<AccessLog> accessLogRepository,
            ILogger<CurriculumsController> logger)
        {
            _curriculumRepository = curriculumRepository;
            _userRepository = userRepository;
            _shortLinkService = shortLinkService;
            _accessLogRepository = accessLogRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os currículos
        /// </summary>
        /// <returns>Lista de currículos</returns>
        /// <response code="200">Retorna a lista de currículos</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CurriculumDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var curriculums = await _curriculumRepository.GetAllAsync();
            var curriculumDtos = curriculums.Select(MapToDto);

            return Ok(ApiResponse<IEnumerable<CurriculumDTO>>.SuccessResponse(curriculumDtos));
        }

        /// <summary>
        /// Obtém os currículos de um usuário específico
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>Lista de currículos do usuário</returns>
        /// <response code="200">Retorna a lista de currículos do usuário</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("user/{userId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CurriculumDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Usuário não encontrado"));
            }

            var curriculums = await _curriculumRepository.GetByUserIdAsync(userId);
            var curriculumDtos = curriculums.Select(MapToDto);

            return Ok(ApiResponse<IEnumerable<CurriculumDTO>>.SuccessResponse(curriculumDtos));
        }

        /// <summary>
        /// Obtém um currículo pelo seu ID
        /// </summary>
        /// <param name="id">ID do currículo</param>
        /// <returns>Currículo encontrado</returns>
        /// <response code="200">Retorna o currículo</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CurriculumDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }

            return Ok(ApiResponse<CurriculumDTO>.SuccessResponse(MapToDto(curriculum)));
        }

        /// <summary>
        /// Obtém um currículo completo pelo seu ID
        /// </summary>
        /// <param name="id">ID do currículo</param>
        /// <returns>Currículo completo encontrado</returns>
        /// <response code="200">Retorna o currículo completo</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpGet("{id:guid}/complete")]
        [ProducesResponseType(typeof(ApiResponse<CurriculumDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompleteById(Guid id)
        {
            var curriculum = await _curriculumRepository.GetCompleteByIdAsync(id);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }

            return Ok(ApiResponse<CurriculumDetailDTO>.SuccessResponse(MapToDetailDto(curriculum)));
        }

        /// <summary>
        /// Cria um novo currículo
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="createCurriculumDto">Dados do currículo</param>
        /// <returns>Currículo criado</returns>
        /// <response code="201">Currículo criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpPost("user/{userId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CurriculumDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(Guid userId, [FromBody] CreateCurriculumDTO createCurriculumDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Usuário não encontrado"));
            }

            var now = DateTime.UtcNow;
            var curriculum = new Curriculum
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = createCurriculumDto.Title,
                Summary = createCurriculumDto.Summary,
                Status = createCurriculumDto.Status,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _curriculumRepository.AddAsync(curriculum);
            await _curriculumRepository.SaveChangesAsync();

            // Gerar automaticamente um link curto para o currículo
            var shortLink = await _shortLinkService.GenerateShortLinkAsync(curriculum.Id);

            _logger.LogInformation("Currículo criado: {CurriculumId} para o usuário: {UserId} com link curto: {ShortLinkHash}",
                curriculum.Id, userId, shortLink.Hash);

            // Obter o currículo atualizado com o link curto
            var updatedCurriculum = await _curriculumRepository.GetCompleteByIdAsync(curriculum.Id);

            return CreatedAtAction(nameof(GetById), new { id = curriculum.Id },
                ApiResponse<CurriculumDTO>.SuccessResponse(MapToDto(updatedCurriculum!), "Currículo criado com sucesso"));
        }

        /// <summary>
        /// Atualiza um currículo existente
        /// </summary>
        /// <param name="id">ID do currículo</param>
        /// <param name="updateCurriculumDto">Dados para atualização</param>
        /// <returns>Currículo atualizado</returns>
        /// <response code="200">Currículo atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CurriculumDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCurriculumDTO updateCurriculumDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }

            // Atualizar título se fornecido
            if (!string.IsNullOrEmpty(updateCurriculumDto.Title))
            {
                curriculum.Title = updateCurriculumDto.Title;
            }

            // Atualizar resumo se fornecido
            if (updateCurriculumDto.Summary != null)
            {
                curriculum.Summary = updateCurriculumDto.Summary;
            }

            // Atualizar status se fornecido
            if (updateCurriculumDto.Status.HasValue)
            {
                curriculum.Status = updateCurriculumDto.Status.Value;
            }

            curriculum.UpdatedAt = DateTime.UtcNow;

            _curriculumRepository.Update(curriculum);
            await _curriculumRepository.SaveChangesAsync();

            _logger.LogInformation("Currículo atualizado: {CurriculumId}", curriculum.Id);

            return Ok(ApiResponse<CurriculumDTO>.SuccessResponse(MapToDto(curriculum), "Currículo atualizado com sucesso"));
        }

        /// <summary>
        /// Remove um currículo
        /// </summary>
        /// <param name="id">ID do currículo</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Currículo removido com sucesso</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }

            _curriculumRepository.Remove(curriculum);
            await _curriculumRepository.SaveChangesAsync();

            _logger.LogInformation("Currículo removido: {CurriculumId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Currículo removido com sucesso"));
        }

        /// <summary>
        /// Acessa um currículo através de um link curto
        /// </summary>
        /// <param name="hash">Hash do link curto</param>
        /// <returns>Currículo associado ao link curto</returns>
        /// <response code="200">Retorna o currículo</response>
        /// <response code="404">Link curto não encontrado ou revogado</response>
        [HttpGet("shortlink/{hash}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<CurriculumDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AccessByShortLink(string hash)
        {
            var curriculum = await _shortLinkService.GetCurriculumByHashAsync(hash);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Link curto não encontrado ou revogado"));
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

            _logger.LogInformation("Acesso ao currículo via link curto: {Hash}", hash);

            return Ok(ApiResponse<CurriculumDetailDTO>.SuccessResponse(MapToDetailDto(curriculum)));
        }

        /// <summary>
        /// Converte um modelo Curriculum para CurriculumDTO
        /// </summary>
        /// <param name="curriculum">Modelo Curriculum</param>
        /// <returns>DTO do currículo</returns>
        private static CurriculumDTO MapToDto(Curriculum curriculum)
        {
            return new CurriculumDTO
            {
                Id = curriculum.Id,
                UserId = curriculum.UserId,
                Title = curriculum.Title,
                Summary = curriculum.Summary,
                Status = curriculum.Status,
                CreatedAt = curriculum.CreatedAt,
                UpdatedAt = curriculum.UpdatedAt
            };
        }

        /// <summary>
        /// Converte um modelo Curriculum para CurriculumDetailDTO
        /// </summary>
        /// <param name="curriculum">Modelo Curriculum</param>
        /// <returns>DTO detalhado do currículo</returns>
        private static CurriculumDetailDTO MapToDetailDto(Curriculum curriculum)
        {
            return new CurriculumDetailDTO
            {
                Id = curriculum.Id,
                UserId = curriculum.UserId,
                Title = curriculum.Title,
                Summary = curriculum.Summary,
                Status = curriculum.Status,
                CreatedAt = curriculum.CreatedAt,
                UpdatedAt = curriculum.UpdatedAt,
                Experiences = curriculum.Experiences.Select(e => new ExperienceDTO
                {
                    Id = e.Id,
                    CurriculumId = e.CurriculumId,
                    CompanyName = e.CompanyName,
                    Role = e.Role,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Location = e.Location
                }).ToList(),
                Educations = curriculum.Educations.Select(e => new EducationDTO
                {
                    Id = e.Id,
                    CurriculumId = e.CurriculumId,
                    Institution = e.Institution,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Description = e.Description
                }).ToList(),
                Skills = curriculum.Skills.Select(s => new SkillDTO
                {
                    Id = s.Id,
                    CurriculumId = s.CurriculumId,
                    TechName = s.TechName,
                    Proficiency = s.Proficiency
                }).ToList(),
                Contacts = curriculum.Contacts.Select(c => new ContactDTO
                {
                    Id = c.Id,
                    CurriculumId = c.CurriculumId,
                    Type = c.Type,
                    Value = c.Value,
                    IsPrimary = c.IsPrimary
                }).ToList(),
                Addresses = curriculum.Addresses.Select(a => new AddressDTO
                {
                    Id = a.Id,
                    CurriculumId = a.CurriculumId,
                    Street = a.Street,
                    Number = a.Number,
                    Complement = a.Complement,
                    Neighborhood = a.Neighborhood,
                    City = a.City,
                    State = a.State,
                    Country = a.Country,
                    ZipCode = a.ZipCode,
                    Type = a.Type
                }).ToList(),
                ShortLinks = curriculum.ShortLinks.Select(s => new ShortLinkDTO
                {
                    Id = s.Id,
                    CurriculumId = s.CurriculumId,
                    Hash = s.Hash,
                    AccessUrl = s.Hash,
                    IsRevoked = s.IsRevoked,
                    CreatedAt = s.CreatedAt,
                    RevokedAt = s.RevokedAt
                }).ToList()
            };
        }
    }
}
