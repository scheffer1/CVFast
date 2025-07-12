using CVFastServices.DTOs;
using CVFastServices.Models;
using CVFastServices.Repositories;
using CVFastServices.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CVFastApi.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de idiomas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly ILogger<LanguagesController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        public LanguagesController(
            ILanguageRepository languageRepository,
            ICurriculumRepository curriculumRepository,
            ILogger<LanguagesController> logger)
        {
            _languageRepository = languageRepository;
            _curriculumRepository = curriculumRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os idiomas
        /// </summary>
        /// <returns>Lista de idiomas</returns>
        /// <response code="200">Retorna a lista de idiomas</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LanguageDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var languages = await _languageRepository.GetAllAsync();
            var languageDtos = languages.Select(MapToDto);

            return Ok(ApiResponse<IEnumerable<LanguageDTO>>.SuccessResponse(languageDtos));
        }

        /// <summary>
        /// Obtém um idioma pelo seu ID
        /// </summary>
        /// <param name="id">ID do idioma</param>
        /// <returns>Idioma encontrado</returns>
        /// <response code="200">Retorna o idioma</response>
        /// <response code="404">Idioma não encontrado</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<LanguageDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var language = await _languageRepository.GetByIdAsync(id);
            if (language == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Idioma não encontrado"));
            }

            return Ok(ApiResponse<LanguageDTO>.SuccessResponse(MapToDto(language)));
        }

        /// <summary>
        /// Obtém todos os idiomas de um currículo específico
        /// </summary>
        /// <param name="curriculumId">ID do currículo</param>
        /// <returns>Lista de idiomas do currículo</returns>
        /// <response code="200">Retorna a lista de idiomas</response>
        [HttpGet("curriculum/{curriculumId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LanguageDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCurriculumId(Guid curriculumId)
        {
            var languages = await _languageRepository.GetByCurriculumIdAsync(curriculumId);
            var languageDtos = languages.Select(MapToDto);

            return Ok(ApiResponse<IEnumerable<LanguageDTO>>.SuccessResponse(languageDtos));
        }

        /// <summary>
        /// Cria um novo idioma
        /// </summary>
        /// <param name="createLanguageDto">Dados do idioma a ser criado</param>
        /// <returns>Idioma criado</returns>
        /// <response code="201">Idioma criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<LanguageDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateLanguageDTO createLanguageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var curriculum = await _curriculumRepository.GetByIdAsync(createLanguageDto.CurriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }

            var language = new Language
            {
                Id = Guid.NewGuid(),
                CurriculumId = createLanguageDto.CurriculumId,
                LanguageName = createLanguageDto.LanguageName,
                Proficiency = createLanguageDto.Proficiency
            };

            await _languageRepository.AddAsync(language);
            await _languageRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            curriculum.UpdatedAt = DateTime.UtcNow;
            _curriculumRepository.Update(curriculum);
            await _curriculumRepository.SaveChangesAsync();
            
            _logger.LogInformation("Idioma criado: {LanguageId} para o currículo: {CurriculumId}", 
                language.Id, language.CurriculumId);
            
            return CreatedAtAction(nameof(GetById), new { id = language.Id }, 
                ApiResponse<LanguageDTO>.SuccessResponse(MapToDto(language), "Idioma criado com sucesso"));
        }

        /// <summary>
        /// Atualiza um idioma existente
        /// </summary>
        /// <param name="id">ID do idioma</param>
        /// <param name="updateLanguageDto">Dados atualizados do idioma</param>
        /// <returns>Idioma atualizado</returns>
        /// <response code="200">Idioma atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Idioma não encontrado</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<LanguageDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLanguageDTO updateLanguageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var language = await _languageRepository.GetByIdAsync(id);
            if (language == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Idioma não encontrado"));
            }

            // Atualizar apenas os campos fornecidos
            if (!string.IsNullOrEmpty(updateLanguageDto.LanguageName))
            {
                language.LanguageName = updateLanguageDto.LanguageName;
            }

            if (updateLanguageDto.Proficiency.HasValue)
            {
                language.Proficiency = updateLanguageDto.Proficiency.Value;
            }

            _languageRepository.Update(language);
            await _languageRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(language.CurriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }
            
            _logger.LogInformation("Idioma atualizado: {LanguageId}", language.Id);

            return Ok(ApiResponse<LanguageDTO>.SuccessResponse(MapToDto(language), "Idioma atualizado com sucesso"));
        }

        /// <summary>
        /// Remove um idioma
        /// </summary>
        /// <param name="id">ID do idioma</param>
        /// <returns>Confirmação da remoção</returns>
        /// <response code="200">Idioma removido com sucesso</response>
        /// <response code="404">Idioma não encontrado</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var language = await _languageRepository.GetByIdAsync(id);
            if (language == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Idioma não encontrado"));
            }

            _languageRepository.Delete(language);
            await _languageRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(language.CurriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }
            
            _logger.LogInformation("Idioma removido: {LanguageId}", language.Id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Idioma removido com sucesso"));
        }

        /// <summary>
        /// Mapeia uma entidade Language para LanguageDTO
        /// </summary>
        /// <param name="language">Entidade Language</param>
        /// <returns>LanguageDTO</returns>
        private static LanguageDTO MapToDto(Language language)
        {
            return new LanguageDTO
            {
                Id = language.Id,
                CurriculumId = language.CurriculumId,
                LanguageName = language.LanguageName,
                Proficiency = language.Proficiency
            };
        }
    }
}
