using CVFastApi.DTOs;
using CVFastApi.Models;
using CVFastApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CVFastApi.Controllers
{
    /// <summary>
    /// Controller para operações relacionadas a formações acadêmicas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EducationsController : ControllerBase
    {
        private readonly IRepository<Education> _educationRepository;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly ILogger<EducationsController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="educationRepository">Repositório de formações acadêmicas</param>
        /// <param name="curriculumRepository">Repositório de currículos</param>
        /// <param name="logger">Logger</param>
        public EducationsController(
            IRepository<Education> educationRepository,
            ICurriculumRepository curriculumRepository,
            ILogger<EducationsController> logger)
        {
            _educationRepository = educationRepository;
            _curriculumRepository = curriculumRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todas as formações acadêmicas
        /// </summary>
        /// <returns>Lista de formações acadêmicas</returns>
        /// <response code="200">Retorna a lista de formações acadêmicas</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EducationDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var educations = await _educationRepository.GetAllAsync();
            var educationDtos = educations.Select(MapToDto);
            
            return Ok(ApiResponse<IEnumerable<EducationDTO>>.SuccessResponse(educationDtos));
        }

        /// <summary>
        /// Obtém as formações acadêmicas de um currículo específico
        /// </summary>
        /// <param name="curriculumId">ID do currículo</param>
        /// <returns>Lista de formações acadêmicas do currículo</returns>
        /// <response code="200">Retorna a lista de formações acadêmicas do currículo</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpGet("curriculum/{curriculumId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EducationDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCurriculumId(Guid curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }
            
            var educations = await _educationRepository.FindAsync(e => e.CurriculumId == curriculumId);
            var educationDtos = educations.Select(MapToDto);
            
            return Ok(ApiResponse<IEnumerable<EducationDTO>>.SuccessResponse(educationDtos));
        }

        /// <summary>
        /// Obtém uma formação acadêmica pelo seu ID
        /// </summary>
        /// <param name="id">ID da formação acadêmica</param>
        /// <returns>Formação acadêmica encontrada</returns>
        /// <response code="200">Retorna a formação acadêmica</response>
        /// <response code="404">Formação acadêmica não encontrada</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<EducationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var education = await _educationRepository.GetByIdAsync(id);
            if (education == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Formação acadêmica não encontrada"));
            }
            
            return Ok(ApiResponse<EducationDTO>.SuccessResponse(MapToDto(education)));
        }

        /// <summary>
        /// Cria uma nova formação acadêmica
        /// </summary>
        /// <param name="createEducationDto">Dados da formação acadêmica</param>
        /// <returns>Formação acadêmica criada</returns>
        /// <response code="201">Formação acadêmica criada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<EducationDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateEducationDTO createEducationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            
            var curriculum = await _curriculumRepository.GetByIdAsync(createEducationDto.CurriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }
            
            var education = new Education
            {
                Id = Guid.NewGuid(),
                CurriculumId = createEducationDto.CurriculumId,
                Institution = createEducationDto.Institution,
                Degree = createEducationDto.Degree,
                FieldOfStudy = createEducationDto.FieldOfStudy,
                StartDate = createEducationDto.StartDate,
                EndDate = createEducationDto.EndDate,
                Description = createEducationDto.Description
            };
            
            await _educationRepository.AddAsync(education);
            await _educationRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            curriculum.UpdatedAt = DateTime.UtcNow;
            _curriculumRepository.Update(curriculum);
            await _curriculumRepository.SaveChangesAsync();
            
            _logger.LogInformation("Formação acadêmica criada: {EducationId} para o currículo: {CurriculumId}", 
                education.Id, education.CurriculumId);
            
            return CreatedAtAction(nameof(GetById), new { id = education.Id }, 
                ApiResponse<EducationDTO>.SuccessResponse(MapToDto(education), "Formação acadêmica criada com sucesso"));
        }

        /// <summary>
        /// Atualiza uma formação acadêmica existente
        /// </summary>
        /// <param name="id">ID da formação acadêmica</param>
        /// <param name="updateEducationDto">Dados para atualização</param>
        /// <returns>Formação acadêmica atualizada</returns>
        /// <response code="200">Formação acadêmica atualizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Formação acadêmica não encontrada</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<EducationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEducationDTO updateEducationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            
            var education = await _educationRepository.GetByIdAsync(id);
            if (education == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Formação acadêmica não encontrada"));
            }
            
            // Atualizar instituição se fornecida
            if (!string.IsNullOrEmpty(updateEducationDto.Institution))
            {
                education.Institution = updateEducationDto.Institution;
            }
            
            // Atualizar grau/curso se fornecido
            if (!string.IsNullOrEmpty(updateEducationDto.Degree))
            {
                education.Degree = updateEducationDto.Degree;
            }
            
            // Atualizar área de estudo se fornecida
            if (!string.IsNullOrEmpty(updateEducationDto.FieldOfStudy))
            {
                education.FieldOfStudy = updateEducationDto.FieldOfStudy;
            }
            
            // Atualizar data de início se fornecida
            if (updateEducationDto.StartDate.HasValue)
            {
                education.StartDate = updateEducationDto.StartDate.Value;
            }
            
            // Atualizar data de término se fornecida
            if (updateEducationDto.EndDate != null)
            {
                education.EndDate = updateEducationDto.EndDate;
            }
            
            // Atualizar descrição se fornecida
            if (updateEducationDto.Description != null)
            {
                education.Description = updateEducationDto.Description;
            }
            
            _educationRepository.Update(education);
            await _educationRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(education.CurriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }
            
            _logger.LogInformation("Formação acadêmica atualizada: {EducationId}", education.Id);
            
            return Ok(ApiResponse<EducationDTO>.SuccessResponse(MapToDto(education), "Formação acadêmica atualizada com sucesso"));
        }

        /// <summary>
        /// Remove uma formação acadêmica
        /// </summary>
        /// <param name="id">ID da formação acadêmica</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Formação acadêmica removida com sucesso</response>
        /// <response code="404">Formação acadêmica não encontrada</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var education = await _educationRepository.GetByIdAsync(id);
            if (education == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Formação acadêmica não encontrada"));
            }
            
            var curriculumId = education.CurriculumId;
            
            _educationRepository.Remove(education);
            await _educationRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }
            
            _logger.LogInformation("Formação acadêmica removida: {EducationId}", id);
            
            return Ok(ApiResponse<object>.SuccessResponse(null, "Formação acadêmica removida com sucesso"));
        }

        /// <summary>
        /// Converte um modelo Education para EducationDTO
        /// </summary>
        /// <param name="education">Modelo Education</param>
        /// <returns>DTO da formação acadêmica</returns>
        private static EducationDTO MapToDto(Education education)
        {
            return new EducationDTO
            {
                Id = education.Id,
                CurriculumId = education.CurriculumId,
                Institution = education.Institution,
                Degree = education.Degree,
                FieldOfStudy = education.FieldOfStudy,
                StartDate = education.StartDate,
                EndDate = education.EndDate,
                Description = education.Description
            };
        }
    }
}
