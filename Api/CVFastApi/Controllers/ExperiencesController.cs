using CVFastServices.DTOs;
using CVFastServices.Models;
using CVFastServices.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVFastApi.Controllers
{
    /// <summary>
    /// Controller para operações relacionadas a experiências profissionais
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ExperiencesController : ControllerBase
    {
        private readonly IRepository<Experience> _experienceRepository;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly ILogger<ExperiencesController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="experienceRepository">Repositório de experiências</param>
        /// <param name="curriculumRepository">Repositório de currículos</param>
        /// <param name="logger">Logger</param>
        public ExperiencesController(
            IRepository<Experience> experienceRepository,
            ICurriculumRepository curriculumRepository,
            ILogger<ExperiencesController> logger)
        {
            _experienceRepository = experienceRepository;
            _curriculumRepository = curriculumRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todas as experiências profissionais
        /// </summary>
        /// <returns>Lista de experiências</returns>
        /// <response code="200">Retorna a lista de experiências</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExperienceDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var experiences = await _experienceRepository.GetAllAsync();
            var experienceDtos = experiences.Select(MapToDto);

            return Ok(ApiResponse<IEnumerable<ExperienceDTO>>.SuccessResponse(experienceDtos));
        }

        /// <summary>
        /// Obtém as experiências profissionais de um currículo específico
        /// </summary>
        /// <param name="curriculumId">ID do currículo</param>
        /// <returns>Lista de experiências do currículo</returns>
        /// <response code="200">Retorna a lista de experiências do currículo</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpGet("curriculum/{curriculumId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExperienceDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCurriculumId(Guid curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }

            var experiences = await _experienceRepository.FindAsync(e => e.CurriculumId == curriculumId);
            var experienceDtos = experiences.Select(MapToDto);

            return Ok(ApiResponse<IEnumerable<ExperienceDTO>>.SuccessResponse(experienceDtos));
        }

        /// <summary>
        /// Obtém uma experiência profissional pelo seu ID
        /// </summary>
        /// <param name="id">ID da experiência</param>
        /// <returns>Experiência encontrada</returns>
        /// <response code="200">Retorna a experiência</response>
        /// <response code="404">Experiência não encontrada</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ExperienceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var experience = await _experienceRepository.GetByIdAsync(id);
            if (experience == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Experiência não encontrada"));
            }

            return Ok(ApiResponse<ExperienceDTO>.SuccessResponse(MapToDto(experience)));
        }

        /// <summary>
        /// Cria uma nova experiência profissional
        /// </summary>
        /// <param name="createExperienceDto">Dados da experiência</param>
        /// <returns>Experiência criada</returns>
        /// <response code="201">Experiência criada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ExperienceDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateExperienceDTO createExperienceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var curriculum = await _curriculumRepository.GetByIdAsync(createExperienceDto.CurriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }

            var experience = new Experience
            {
                Id = Guid.NewGuid(),
                CurriculumId = createExperienceDto.CurriculumId,
                CompanyName = createExperienceDto.CompanyName,
                Role = createExperienceDto.Role,
                Description = createExperienceDto.Description,
                StartDate = createExperienceDto.StartDate,
                EndDate = createExperienceDto.EndDate,
                Location = createExperienceDto.Location
            };

            await _experienceRepository.AddAsync(experience);
            await _experienceRepository.SaveChangesAsync();

            // Atualizar a data de modificação do currículo
            curriculum.UpdatedAt = DateTime.UtcNow;
            _curriculumRepository.Update(curriculum);
            await _curriculumRepository.SaveChangesAsync();

            _logger.LogInformation("Experiência criada: {ExperienceId} para o currículo: {CurriculumId}",
                experience.Id, experience.CurriculumId);

            return CreatedAtAction(nameof(GetById), new { id = experience.Id },
                ApiResponse<ExperienceDTO>.SuccessResponse(MapToDto(experience), "Experiência criada com sucesso"));
        }

        /// <summary>
        /// Atualiza uma experiência profissional existente
        /// </summary>
        /// <param name="id">ID da experiência</param>
        /// <param name="updateExperienceDto">Dados para atualização</param>
        /// <returns>Experiência atualizada</returns>
        /// <response code="200">Experiência atualizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Experiência não encontrada</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ExperienceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExperienceDTO updateExperienceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var experience = await _experienceRepository.GetByIdAsync(id);
            if (experience == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Experiência não encontrada"));
            }

            // Atualizar nome da empresa se fornecido
            if (!string.IsNullOrEmpty(updateExperienceDto.CompanyName))
            {
                experience.CompanyName = updateExperienceDto.CompanyName;
            }

            // Atualizar cargo se fornecido
            if (!string.IsNullOrEmpty(updateExperienceDto.Role))
            {
                experience.Role = updateExperienceDto.Role;
            }

            // Atualizar descrição se fornecida
            if (updateExperienceDto.Description != null)
            {
                experience.Description = updateExperienceDto.Description;
            }

            // Atualizar data de início se fornecida
            if (updateExperienceDto.StartDate.HasValue)
            {
                experience.StartDate = updateExperienceDto.StartDate.Value;
            }

            // Atualizar data de término se fornecida
            if (updateExperienceDto.EndDate != null)
            {
                experience.EndDate = updateExperienceDto.EndDate;
            }

            // Atualizar localização se fornecida
            if (updateExperienceDto.Location != null)
            {
                experience.Location = updateExperienceDto.Location;
            }

            _experienceRepository.Update(experience);
            await _experienceRepository.SaveChangesAsync();

            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(experience.CurriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }

            _logger.LogInformation("Experiência atualizada: {ExperienceId}", experience.Id);

            return Ok(ApiResponse<ExperienceDTO>.SuccessResponse(MapToDto(experience), "Experiência atualizada com sucesso"));
        }

        /// <summary>
        /// Remove uma experiência profissional
        /// </summary>
        /// <param name="id">ID da experiência</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Experiência removida com sucesso</response>
        /// <response code="404">Experiência não encontrada</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var experience = await _experienceRepository.GetByIdAsync(id);
            if (experience == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Experiência não encontrada"));
            }

            var curriculumId = experience.CurriculumId;

            _experienceRepository.Remove(experience);
            await _experienceRepository.SaveChangesAsync();

            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }

            _logger.LogInformation("Experiência removida: {ExperienceId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Experiência removida com sucesso"));
        }

        /// <summary>
        /// Converte um modelo Experience para ExperienceDTO
        /// </summary>
        /// <param name="experience">Modelo Experience</param>
        /// <returns>DTO da experiência</returns>
        private static ExperienceDTO MapToDto(Experience experience)
        {
            return new ExperienceDTO
            {
                Id = experience.Id,
                CurriculumId = experience.CurriculumId,
                CompanyName = experience.CompanyName,
                Role = experience.Role,
                Description = experience.Description,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                Location = experience.Location
            };
        }
    }
}
