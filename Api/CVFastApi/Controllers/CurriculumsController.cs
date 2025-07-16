using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CVFastServices.Data;
using CVFastServices.DTOs;
using CVFastServices.Models;
using CVFastServices.Repositories;
using CVFastServices.Repositories.Interfaces;
using CVFastServices.Services.Interfaces;
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
        private readonly IRepository<Experience> _experienceRepository;
        private readonly IRepository<Education> _educationRepository;
        private readonly IRepository<Skill> _skillRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IRepository<Contact> _contactRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CurriculumsController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="curriculumRepository">Repositório de currículos</param>
        /// <param name="userRepository">Repositório de usuários</param>
        /// <param name="shortLinkService">Serviço de links curtos</param>
        /// <param name="accessLogRepository">Repositório de logs de acesso</param>
        /// <param name="experienceRepository">Repositório de experiências</param>
        /// <param name="educationRepository">Repositório de educações</param>
        /// <param name="skillRepository">Repositório de habilidades</param>
        /// <param name="languageRepository">Repositório de idiomas</param>
        /// <param name="contactRepository">Repositório de contatos</param>
        /// <param name="addressRepository">Repositório de endereços</param>
        /// <param name="context">Contexto do banco de dados</param>
        /// <param name="logger">Logger</param>
        public CurriculumsController(
            ICurriculumRepository curriculumRepository,
            IUserRepository userRepository,
            IShortLinkService shortLinkService,
            IRepository<AccessLog> accessLogRepository,
            IRepository<Experience> experienceRepository,
            IRepository<Education> educationRepository,
            IRepository<Skill> skillRepository,
            ILanguageRepository languageRepository,
            IRepository<Contact> contactRepository,
            IRepository<Address> addressRepository,
            ApplicationDbContext context,
            ILogger<CurriculumsController> logger)
        {
            _curriculumRepository = curriculumRepository;
            _userRepository = userRepository;
            _shortLinkService = shortLinkService;
            _accessLogRepository = accessLogRepository;
            _experienceRepository = experienceRepository;
            _educationRepository = educationRepository;
            _skillRepository = skillRepository;
            _languageRepository = languageRepository;
            _contactRepository = contactRepository;
            _addressRepository = addressRepository;
            _context = context;
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
        /// Obtém os currículos de um usuário específico com informações básicas incluindo shortLinks
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>Lista de currículos do usuário</returns>
        /// <response code="200">Retorna a lista de currículos do usuário</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("user/{userId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CurriculumWithShortLinksDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Usuário não encontrado"));
            }

            var curriculums = await _curriculumRepository.GetByUserIdAsync(userId);
            var curriculumDtos = new List<CurriculumWithShortLinksDTO>();

            foreach (var curriculum in curriculums)
            {
                // Carregar o currículo completo para obter os shortLinks
                var completeCurriculum = await _curriculumRepository.GetCompleteByIdAsync(curriculum.Id);
                if (completeCurriculum != null)
                {
                    curriculumDtos.Add(MapToCurriculumWithShortLinksDto(completeCurriculum));
                }
            }

            return Ok(ApiResponse<IEnumerable<CurriculumWithShortLinksDTO>>.SuccessResponse(curriculumDtos));
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
        /// Cria um currículo completo com todas as seções de uma vez
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="createCompleteCurriculumDto">Dados completos do currículo</param>
        /// <returns>Currículo completo criado</returns>
        /// <response code="201">Currículo completo criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpPost("user/{userId:guid}/complete")]
        [ProducesResponseType(typeof(ApiResponse<CurriculumDetailDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateComplete(Guid userId, [FromBody] CreateCompleteCurriculumDTO createCompleteCurriculumDto)
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

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Criar o currículo principal
                var curriculum = new Curriculum
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = createCompleteCurriculumDto.Title,
                    Summary = createCompleteCurriculumDto.Summary,
                    Status = createCompleteCurriculumDto.Status,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _curriculumRepository.AddAsync(curriculum);
                await _curriculumRepository.SaveChangesAsync();

                // 2. Criar experiências
                foreach (var expDto in createCompleteCurriculumDto.Experiences)
                {
                    var experience = new Experience
                    {
                        Id = Guid.NewGuid(),
                        CurriculumId = curriculum.Id,
                        CompanyName = expDto.CompanyName,
                        Role = expDto.Role,
                        Description = expDto.Description,
                        StartDate = expDto.StartDate,
                        EndDate = expDto.EndDate,
                        Location = expDto.Location
                    };
                    await _experienceRepository.AddAsync(experience);
                }

                // 3. Criar educações
                foreach (var eduDto in createCompleteCurriculumDto.Educations)
                {
                    var education = new Education
                    {
                        Id = Guid.NewGuid(),
                        CurriculumId = curriculum.Id,
                        Institution = eduDto.Institution,
                        Degree = eduDto.Degree,
                        FieldOfStudy = eduDto.FieldOfStudy,
                        StartDate = eduDto.StartDate,
                        EndDate = eduDto.EndDate,
                        Description = eduDto.Description
                    };
                    await _educationRepository.AddAsync(education);
                }

                // 4. Criar habilidades
                foreach (var skillDto in createCompleteCurriculumDto.Skills)
                {
                    var skill = new Skill
                    {
                        Id = Guid.NewGuid(),
                        CurriculumId = curriculum.Id,
                        TechName = skillDto.TechName,
                        Proficiency = skillDto.Proficiency
                    };
                    await _skillRepository.AddAsync(skill);
                }

                // 5. Criar idiomas
                foreach (var languageDto in createCompleteCurriculumDto.Languages)
                {
                    var language = new Language
                    {
                        Id = Guid.NewGuid(),
                        CurriculumId = curriculum.Id,
                        LanguageName = languageDto.LanguageName,
                        Proficiency = languageDto.Proficiency
                    };
                    await _languageRepository.AddAsync(language);
                }

                // 6. Criar contatos
                foreach (var contactDto in createCompleteCurriculumDto.Contacts)
                {
                    var contact = new Contact
                    {
                        Id = Guid.NewGuid(),
                        CurriculumId = curriculum.Id,
                        Type = contactDto.Type,
                        Value = contactDto.Value,
                        IsPrimary = contactDto.IsPrimary
                    };
                    await _contactRepository.AddAsync(contact);
                }

                // 6. Criar endereços
                foreach (var addressDto in createCompleteCurriculumDto.Addresses)
                {
                    var address = new Address
                    {
                        Id = Guid.NewGuid(),
                        CurriculumId = curriculum.Id,
                        Street = addressDto.Street,
                        Number = addressDto.Number,
                        Complement = addressDto.Complement,
                        Neighborhood = addressDto.Neighborhood,
                        City = addressDto.City,
                        State = addressDto.State,
                        Country = addressDto.Country,
                        ZipCode = addressDto.ZipCode,
                        Type = addressDto.Type
                    };
                    await _addressRepository.AddAsync(address);
                }

                // Salvar todas as alterações
                await _experienceRepository.SaveChangesAsync();
                await _educationRepository.SaveChangesAsync();
                await _skillRepository.SaveChangesAsync();
                await _contactRepository.SaveChangesAsync();
                await _addressRepository.SaveChangesAsync();

                // 7. Gerar automaticamente um link curto para o currículo
                var shortLink = await _shortLinkService.GenerateShortLinkAsync(curriculum.Id);

                await transaction.CommitAsync();

                _logger.LogInformation("Currículo completo criado: {CurriculumId} para o usuário: {UserId} com link curto: {ShortLinkHash}",
                    curriculum.Id, userId, shortLink.Hash);

                // Obter o currículo completo atualizado
                var completeCurriculum = await _curriculumRepository.GetCompleteByIdAsync(curriculum.Id);

                return CreatedAtAction(nameof(GetCompleteById), new { id = curriculum.Id },
                    ApiResponse<CurriculumDetailDTO>.SuccessResponse(MapToDetailDto(completeCurriculum!), "Currículo completo criado com sucesso"));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao criar currículo completo para o usuário: {UserId}", userId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno do servidor"));
            }
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

            // Verificar visibilidade do currículo
            if (curriculum.Status == CurriculumStatus.Hidden)
            {
                // Se o currículo está oculto, verificar se o usuário autenticado é o dono
                var currentUserId = await GetCurrentUserIdAsync();
                if (currentUserId == null || currentUserId != curriculum.UserId)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Link curto não encontrado ou revogado"));
                }
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
        /// Altera a visibilidade de um currículo (público/privado)
        /// </summary>
        /// <param name="id">ID do currículo</param>
        /// <param name="visibilityDto">Dados de visibilidade</param>
        /// <returns>Currículo atualizado</returns>
        /// <response code="200">Visibilidade alterada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Currículo não encontrado</response>
        /// <response code="403">Usuário não autorizado</response>
        [HttpPatch("{id}/visibility")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CurriculumDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateVisibility(Guid id, [FromBody] UpdateCurriculumVisibilityDTO visibilityDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var currentUserId = await GetCurrentUserIdAsync();
            if (currentUserId == null)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Usuário não autenticado"));
            }

            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }

            if (curriculum.UserId != currentUserId)
            {
                return Forbid();
            }

            // Atualizar o status baseado na visibilidade
            curriculum.Status = visibilityDto.IsPublic ? CurriculumStatus.Active : CurriculumStatus.Hidden;
            curriculum.UpdatedAt = DateTime.UtcNow;

            _curriculumRepository.Update(curriculum);
            await _curriculumRepository.SaveChangesAsync();

            _logger.LogInformation("Visibilidade do currículo alterada: {CurriculumId} para {Status} pelo usuário: {UserId}",
                curriculum.Id, curriculum.Status, currentUserId);

            return Ok(ApiResponse<CurriculumDTO>.SuccessResponse(MapToDto(curriculum), "Visibilidade alterada com sucesso"));
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
                UserName = curriculum.User?.Name ?? "Usuário",
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
                Languages = curriculum.Languages.Select(l => new LanguageDTO
                {
                    Id = l.Id,
                    CurriculumId = l.CurriculumId,
                    LanguageName = l.LanguageName,
                    Proficiency = l.Proficiency
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

        /// <summary>
        /// Converte um modelo Curriculum para CurriculumWithShortLinksDTO
        /// </summary>
        /// <param name="curriculum">Modelo Curriculum</param>
        /// <returns>DTO do currículo com shortLinks</returns>
        private static CurriculumWithShortLinksDTO MapToCurriculumWithShortLinksDto(Curriculum curriculum)
        {
            return new CurriculumWithShortLinksDTO
            {
                Id = curriculum.Id,
                UserId = curriculum.UserId,
                Title = curriculum.Title,
                Summary = curriculum.Summary,
                Status = curriculum.Status,
                CreatedAt = curriculum.CreatedAt,
                UpdatedAt = curriculum.UpdatedAt,
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

        /// <summary>
        /// Obtém o ID do usuário atual a partir do token JWT
        /// </summary>
        /// <returns>ID do usuário ou null se não autenticado</returns>
        private async Task<Guid?> GetCurrentUserIdAsync()
        {
            if (!HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                _logger.LogWarning("Usuário não autenticado");
                return null;
            }

            // Tentar obter o ID do claim nameidentifier (padrão do ASP.NET Core)
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogInformation("UserId obtido do claim nameidentifier: {UserId}", userId);
                return userId;
            }

            // Fallback: tentar pelo claim Sub (JWT padrão)
            userIdClaim = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out userId))
            {
                _logger.LogInformation("UserId obtido do claim Sub: {UserId}", userId);
                return userId;
            }

            // Se não conseguir pelos claims de ID, tentar pelo email
            var emailClaim = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
            if (!string.IsNullOrEmpty(emailClaim))
            {
                _logger.LogInformation("Buscando usuário pelo email: {Email}", emailClaim);
                var user = await _userRepository.GetByEmailAsync(emailClaim);
                if (user != null)
                {
                    _logger.LogInformation("Usuário encontrado pelo email: {UserId}", user.Id);
                    return user.Id;
                }
            }

            _logger.LogWarning("Não foi possível obter o ID do usuário. Claims disponíveis: {Claims}",
                string.Join(", ", HttpContext.User.Claims.Select(c => $"{c.Type}={c.Value}")));
            return null;
        }
    }
}
