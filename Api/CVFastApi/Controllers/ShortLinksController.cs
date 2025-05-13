using System.Security.Cryptography;
using System.Text;
using CVFastApi.DTOs;
using CVFastApi.Models;
using CVFastApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVFastApi.Controllers
{
    /// <summary>
    /// Controller para operações relacionadas a links curtos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ShortLinksController : ControllerBase
    {
        private readonly IShortLinkRepository _shortLinkRepository;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly IRepository<AccessLog> _accessLogRepository;
        private readonly ILogger<ShortLinksController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="shortLinkRepository">Repositório de links curtos</param>
        /// <param name="curriculumRepository">Repositório de currículos</param>
        /// <param name="accessLogRepository">Repositório de logs de acesso</param>
        /// <param name="logger">Logger</param>
        public ShortLinksController(
            IShortLinkRepository shortLinkRepository,
            ICurriculumRepository curriculumRepository,
            IRepository<AccessLog> accessLogRepository,
            ILogger<ShortLinksController> logger)
        {
            _shortLinkRepository = shortLinkRepository;
            _curriculumRepository = curriculumRepository;
            _accessLogRepository = accessLogRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os links curtos
        /// </summary>
        /// <returns>Lista de links curtos</returns>
        /// <response code="200">Retorna a lista de links curtos</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ShortLinkDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var shortLinks = await _shortLinkRepository.GetAllAsync();
            var shortLinkDtos = shortLinks.Select(MapToDto);

            return Ok(ApiResponse<IEnumerable<ShortLinkDTO>>.SuccessResponse(shortLinkDtos));
        }

        /// <summary>
        /// Obtém os links curtos de um currículo específico
        /// </summary>
        /// <param name="curriculumId">ID do currículo</param>
        /// <returns>Lista de links curtos do currículo</returns>
        /// <response code="200">Retorna a lista de links curtos do currículo</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpGet("curriculum/{curriculumId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ShortLinkDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCurriculumId(Guid curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }

            var shortLinks = await _shortLinkRepository.FindAsync(s => s.CurriculumId == curriculumId);
            var shortLinkDtos = shortLinks.Select(MapToDto);

            return Ok(ApiResponse<IEnumerable<ShortLinkDTO>>.SuccessResponse(shortLinkDtos));
        }

        /// <summary>
        /// Obtém um link curto pelo seu ID
        /// </summary>
        /// <param name="id">ID do link curto</param>
        /// <returns>Link curto encontrado</returns>
        /// <response code="200">Retorna o link curto</response>
        /// <response code="404">Link curto não encontrado</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ShortLinkDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var shortLink = await _shortLinkRepository.GetByIdAsync(id);
            if (shortLink == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Link curto não encontrado"));
            }

            return Ok(ApiResponse<ShortLinkDTO>.SuccessResponse(MapToDto(shortLink)));
        }

        /// <summary>
        /// Acessa um currículo através de um link curto
        /// </summary>
        /// <param name="hash">Hash do link curto</param>
        /// <returns>Currículo associado ao link curto</returns>
        /// <response code="200">Retorna o currículo</response>
        /// <response code="404">Link curto não encontrado ou revogado</response>
        [HttpGet("access/{hash}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<CurriculumDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AccessByHash(string hash)
        {
            var shortLink = await _shortLinkRepository.GetActiveByHashAsync(hash);
            if (shortLink == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Link curto não encontrado ou revogado"));
            }

            // Registrar o acesso
            var accessLog = new AccessLog
            {
                Id = Guid.NewGuid(),
                ShortLinkId = shortLink.Id,
                IP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
                AccessedAt = DateTime.UtcNow
            };

            await _accessLogRepository.AddAsync(accessLog);
            await _accessLogRepository.SaveChangesAsync();

            _logger.LogInformation("Acesso ao currículo via link curto: {ShortLinkId}, IP: {IP}",
                shortLink.Id, accessLog.IP);

            // Obter o currículo completo
            var curriculum = await _curriculumRepository.GetCompleteByIdAsync(shortLink.CurriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }

            // Mapear para DTO
            var curriculumDto = new CurriculumDetailDTO
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
                ShortLinks = curriculum.ShortLinks.Where(s => !s.IsRevoked).Select(s => new ShortLinkDTO
                {
                    Id = s.Id,
                    CurriculumId = s.CurriculumId,
                    Hash = s.Hash,
                    FullUrl = $"{Request.Scheme}://{Request.Host}/api/shortlinks/access/{s.Hash}",
                    IsRevoked = s.IsRevoked,
                    CreatedAt = s.CreatedAt,
                    RevokedAt = s.RevokedAt
                }).ToList()
            };

            return Ok(ApiResponse<CurriculumDetailDTO>.SuccessResponse(curriculumDto));
        }

        /// <summary>
        /// Cria um novo link curto
        /// </summary>
        /// <param name="createShortLinkDto">Dados do link curto</param>
        /// <returns>Link curto criado</returns>
        /// <response code="201">Link curto criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ShortLinkDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateShortLinkDTO createShortLinkDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var curriculum = await _curriculumRepository.GetByIdAsync(createShortLinkDto.CurriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }

            // Gerar hash único
            var hash = GenerateUniqueHash(createShortLinkDto.CurriculumId);

            var shortLink = new ShortLink
            {
                Id = Guid.NewGuid(),
                CurriculumId = createShortLinkDto.CurriculumId,
                Hash = hash,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _shortLinkRepository.AddAsync(shortLink);
            await _shortLinkRepository.SaveChangesAsync();

            // Atualizar a data de modificação do currículo
            curriculum.UpdatedAt = DateTime.UtcNow;
            _curriculumRepository.Update(curriculum);
            await _curriculumRepository.SaveChangesAsync();

            _logger.LogInformation("Link curto criado: {ShortLinkId} para o currículo: {CurriculumId}",
                shortLink.Id, shortLink.CurriculumId);

            return CreatedAtAction(nameof(GetById), new { id = shortLink.Id },
                ApiResponse<ShortLinkDTO>.SuccessResponse(MapToDto(shortLink), "Link curto criado com sucesso"));
        }

        /// <summary>
        /// Revoga um link curto
        /// </summary>
        /// <param name="id">ID do link curto</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Link curto revogado com sucesso</response>
        /// <response code="404">Link curto não encontrado</response>
        [HttpPut("{id:guid}/revoke")]
        [ProducesResponseType(typeof(ApiResponse<ShortLinkDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Revoke(Guid id)
        {
            var success = await _shortLinkRepository.RevokeAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Link curto não encontrado"));
            }

            await _shortLinkRepository.SaveChangesAsync();

            var shortLink = await _shortLinkRepository.GetByIdAsync(id);
            if (shortLink != null)
            {
                // Atualizar a data de modificação do currículo
                var curriculum = await _curriculumRepository.GetByIdAsync(shortLink.CurriculumId);
                if (curriculum != null)
                {
                    curriculum.UpdatedAt = DateTime.UtcNow;
                    _curriculumRepository.Update(curriculum);
                    await _curriculumRepository.SaveChangesAsync();
                }

                _logger.LogInformation("Link curto revogado: {ShortLinkId}", id);

                return Ok(ApiResponse<ShortLinkDTO>.SuccessResponse(MapToDto(shortLink), "Link curto revogado com sucesso"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Link curto revogado com sucesso"));
        }

        /// <summary>
        /// Obtém os logs de acesso de um link curto
        /// </summary>
        /// <param name="id">ID do link curto</param>
        /// <returns>Lista de logs de acesso</returns>
        /// <response code="200">Retorna a lista de logs de acesso</response>
        /// <response code="404">Link curto não encontrado</response>
        [HttpGet("{id:guid}/logs")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AccessLogDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLogs(Guid id)
        {
            var shortLink = await _shortLinkRepository.GetByIdAsync(id);
            if (shortLink == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Link curto não encontrado"));
            }

            var logs = await _accessLogRepository.FindAsync(l => l.ShortLinkId == id);
            var logDtos = logs.Select(l => new AccessLogDTO
            {
                Id = l.Id,
                ShortLinkId = l.ShortLinkId,
                IP = l.IP,
                UserAgent = l.UserAgent,
                AccessedAt = l.AccessedAt
            });

            return Ok(ApiResponse<IEnumerable<AccessLogDTO>>.SuccessResponse(logDtos));
        }

        /// <summary>
        /// Gera um hash único para um link curto
        /// </summary>
        /// <param name="curriculumId">ID do currículo</param>
        /// <returns>Hash gerado</returns>
        private static string GenerateUniqueHash(Guid curriculumId)
        {
            // Combinar o ID do currículo com um timestamp e um valor aleatório
            var input = $"{curriculumId}_{DateTime.UtcNow.Ticks}_{Guid.NewGuid()}";

            // Gerar hash usando SHA256
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Converter para Base64 e remover caracteres especiais
            var base64 = Convert.ToBase64String(hashBytes)
                .Replace("/", "_")
                .Replace("+", "-")
                .Replace("=", "");

            // Retornar apenas os primeiros 8 caracteres
            return base64[..8];
        }

        /// <summary>
        /// Converte um modelo ShortLink para ShortLinkDTO
        /// </summary>
        /// <param name="shortLink">Modelo ShortLink</param>
        /// <returns>DTO do link curto</returns>
        private ShortLinkDTO MapToDto(ShortLink shortLink)
        {
            return new ShortLinkDTO
            {
                Id = shortLink.Id,
                CurriculumId = shortLink.CurriculumId,
                Hash = shortLink.Hash,
                FullUrl = $"{Request.Scheme}://{Request.Host}/api/shortlinks/access/{shortLink.Hash}",
                IsRevoked = shortLink.IsRevoked,
                CreatedAt = shortLink.CreatedAt,
                RevokedAt = shortLink.RevokedAt
            };
        }
    }
}
