using CVFastServices.DTOs;
using CVFastServices.Models;
using CVFastServices.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CVFastApi.Controllers
{
    /// <summary>
    /// Controller para operações relacionadas a habilidades técnicas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SkillsController : ControllerBase
    {
        private readonly IRepository<Skill> _skillRepository;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly ILogger<SkillsController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="skillRepository">Repositório de habilidades</param>
        /// <param name="curriculumRepository">Repositório de currículos</param>
        /// <param name="logger">Logger</param>
        public SkillsController(
            IRepository<Skill> skillRepository,
            ICurriculumRepository curriculumRepository,
            ILogger<SkillsController> logger)
        {
            _skillRepository = skillRepository;
            _curriculumRepository = curriculumRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todas as habilidades técnicas
        /// </summary>
        /// <returns>Lista de habilidades</returns>
        /// <response code="200">Retorna a lista de habilidades</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SkillDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var skills = await _skillRepository.GetAllAsync();
            var skillDtos = skills.Select(MapToDto);
            
            return Ok(ApiResponse<IEnumerable<SkillDTO>>.SuccessResponse(skillDtos));
        }

        /// <summary>
        /// Obtém as habilidades técnicas de um currículo específico
        /// </summary>
        /// <param name="curriculumId">ID do currículo</param>
        /// <returns>Lista de habilidades do currículo</returns>
        /// <response code="200">Retorna a lista de habilidades do currículo</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpGet("curriculum/{curriculumId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SkillDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCurriculumId(Guid curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }
            
            var skills = await _skillRepository.FindAsync(s => s.CurriculumId == curriculumId);
            var skillDtos = skills.Select(MapToDto);
            
            return Ok(ApiResponse<IEnumerable<SkillDTO>>.SuccessResponse(skillDtos));
        }

        /// <summary>
        /// Obtém uma habilidade técnica pelo seu ID
        /// </summary>
        /// <param name="id">ID da habilidade</param>
        /// <returns>Habilidade encontrada</returns>
        /// <response code="200">Retorna a habilidade</response>
        /// <response code="404">Habilidade não encontrada</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<SkillDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var skill = await _skillRepository.GetByIdAsync(id);
            if (skill == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Habilidade não encontrada"));
            }
            
            return Ok(ApiResponse<SkillDTO>.SuccessResponse(MapToDto(skill)));
        }

        /// <summary>
        /// Cria uma nova habilidade técnica
        /// </summary>
        /// <param name="createSkillDto">Dados da habilidade</param>
        /// <returns>Habilidade criada</returns>
        /// <response code="201">Habilidade criada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SkillDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateSkillDTO createSkillDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            
            var curriculum = await _curriculumRepository.GetByIdAsync(createSkillDto.CurriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }
            
            var skill = new Skill
            {
                Id = Guid.NewGuid(),
                CurriculumId = createSkillDto.CurriculumId,
                TechName = createSkillDto.TechName,
                Proficiency = createSkillDto.Proficiency
            };
            
            await _skillRepository.AddAsync(skill);
            await _skillRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            curriculum.UpdatedAt = DateTime.UtcNow;
            _curriculumRepository.Update(curriculum);
            await _curriculumRepository.SaveChangesAsync();
            
            _logger.LogInformation("Habilidade criada: {SkillId} para o currículo: {CurriculumId}", 
                skill.Id, skill.CurriculumId);
            
            return CreatedAtAction(nameof(GetById), new { id = skill.Id }, 
                ApiResponse<SkillDTO>.SuccessResponse(MapToDto(skill), "Habilidade criada com sucesso"));
        }

        /// <summary>
        /// Atualiza uma habilidade técnica existente
        /// </summary>
        /// <param name="id">ID da habilidade</param>
        /// <param name="updateSkillDto">Dados para atualização</param>
        /// <returns>Habilidade atualizada</returns>
        /// <response code="200">Habilidade atualizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Habilidade não encontrada</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<SkillDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSkillDTO updateSkillDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            
            var skill = await _skillRepository.GetByIdAsync(id);
            if (skill == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Habilidade não encontrada"));
            }
            
            // Atualizar nome da tecnologia se fornecido
            if (!string.IsNullOrEmpty(updateSkillDto.TechName))
            {
                skill.TechName = updateSkillDto.TechName;
            }
            
            // Atualizar nível de proficiência se fornecido
            if (updateSkillDto.Proficiency.HasValue)
            {
                skill.Proficiency = updateSkillDto.Proficiency.Value;
            }
            
            _skillRepository.Update(skill);
            await _skillRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(skill.CurriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }
            
            _logger.LogInformation("Habilidade atualizada: {SkillId}", skill.Id);
            
            return Ok(ApiResponse<SkillDTO>.SuccessResponse(MapToDto(skill), "Habilidade atualizada com sucesso"));
        }

        /// <summary>
        /// Remove uma habilidade técnica
        /// </summary>
        /// <param name="id">ID da habilidade</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Habilidade removida com sucesso</response>
        /// <response code="404">Habilidade não encontrada</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var skill = await _skillRepository.GetByIdAsync(id);
            if (skill == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Habilidade não encontrada"));
            }
            
            var curriculumId = skill.CurriculumId;
            
            _skillRepository.Remove(skill);
            await _skillRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }
            
            _logger.LogInformation("Habilidade removida: {SkillId}", id);
            
            return Ok(ApiResponse<object>.SuccessResponse(null, "Habilidade removida com sucesso"));
        }

        /// <summary>
        /// Converte um modelo Skill para SkillDTO
        /// </summary>
        /// <param name="skill">Modelo Skill</param>
        /// <returns>DTO da habilidade</returns>
        private static SkillDTO MapToDto(Skill skill)
        {
            return new SkillDTO
            {
                Id = skill.Id,
                CurriculumId = skill.CurriculumId,
                TechName = skill.TechName,
                Proficiency = skill.Proficiency
            };
        }
    }
}
