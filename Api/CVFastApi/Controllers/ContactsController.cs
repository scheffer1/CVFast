using CVFastApi.DTOs;
using CVFastApi.Models;
using CVFastApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CVFastApi.Controllers
{
    /// <summary>
    /// Controller para operações relacionadas a contatos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ContactsController : ControllerBase
    {
        private readonly IRepository<Contact> _contactRepository;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly ILogger<ContactsController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="contactRepository">Repositório de contatos</param>
        /// <param name="curriculumRepository">Repositório de currículos</param>
        /// <param name="logger">Logger</param>
        public ContactsController(
            IRepository<Contact> contactRepository,
            ICurriculumRepository curriculumRepository,
            ILogger<ContactsController> logger)
        {
            _contactRepository = contactRepository;
            _curriculumRepository = curriculumRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os contatos
        /// </summary>
        /// <returns>Lista de contatos</returns>
        /// <response code="200">Retorna a lista de contatos</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContactDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var contacts = await _contactRepository.GetAllAsync();
            var contactDtos = contacts.Select(MapToDto);
            
            return Ok(ApiResponse<IEnumerable<ContactDTO>>.SuccessResponse(contactDtos));
        }

        /// <summary>
        /// Obtém os contatos de um currículo específico
        /// </summary>
        /// <param name="curriculumId">ID do currículo</param>
        /// <returns>Lista de contatos do currículo</returns>
        /// <response code="200">Retorna a lista de contatos do currículo</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpGet("curriculum/{curriculumId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContactDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCurriculumId(Guid curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }
            
            var contacts = await _contactRepository.FindAsync(c => c.CurriculumId == curriculumId);
            var contactDtos = contacts.Select(MapToDto);
            
            return Ok(ApiResponse<IEnumerable<ContactDTO>>.SuccessResponse(contactDtos));
        }

        /// <summary>
        /// Obtém um contato pelo seu ID
        /// </summary>
        /// <param name="id">ID do contato</param>
        /// <returns>Contato encontrado</returns>
        /// <response code="200">Retorna o contato</response>
        /// <response code="404">Contato não encontrado</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ContactDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Contato não encontrado"));
            }
            
            return Ok(ApiResponse<ContactDTO>.SuccessResponse(MapToDto(contact)));
        }

        /// <summary>
        /// Cria um novo contato
        /// </summary>
        /// <param name="createContactDto">Dados do contato</param>
        /// <returns>Contato criado</returns>
        /// <response code="201">Contato criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ContactDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateContactDTO createContactDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            
            var curriculum = await _curriculumRepository.GetByIdAsync(createContactDto.CurriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }
            
            // Se for marcado como primário, desmarcar outros contatos do mesmo tipo
            if (createContactDto.IsPrimary)
            {
                var existingPrimaryContacts = await _contactRepository.FindAsync(c => 
                    c.CurriculumId == createContactDto.CurriculumId && 
                    c.Type == createContactDto.Type && 
                    c.IsPrimary);
                
                foreach (var existingContact in existingPrimaryContacts)
                {
                    existingContact.IsPrimary = false;
                    _contactRepository.Update(existingContact);
                }
            }
            
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                CurriculumId = createContactDto.CurriculumId,
                Type = createContactDto.Type,
                Value = createContactDto.Value,
                IsPrimary = createContactDto.IsPrimary
            };
            
            await _contactRepository.AddAsync(contact);
            await _contactRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            curriculum.UpdatedAt = DateTime.UtcNow;
            _curriculumRepository.Update(curriculum);
            await _curriculumRepository.SaveChangesAsync();
            
            _logger.LogInformation("Contato criado: {ContactId} para o currículo: {CurriculumId}", 
                contact.Id, contact.CurriculumId);
            
            return CreatedAtAction(nameof(GetById), new { id = contact.Id }, 
                ApiResponse<ContactDTO>.SuccessResponse(MapToDto(contact), "Contato criado com sucesso"));
        }

        /// <summary>
        /// Atualiza um contato existente
        /// </summary>
        /// <param name="id">ID do contato</param>
        /// <param name="updateContactDto">Dados para atualização</param>
        /// <returns>Contato atualizado</returns>
        /// <response code="200">Contato atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Contato não encontrado</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ContactDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactDTO updateContactDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Contato não encontrado"));
            }
            
            // Se for marcado como primário, desmarcar outros contatos do mesmo tipo
            if (updateContactDto.IsPrimary.HasValue && updateContactDto.IsPrimary.Value && !contact.IsPrimary)
            {
                var contactType = updateContactDto.Type ?? contact.Type;
                var existingPrimaryContacts = await _contactRepository.FindAsync(c => 
                    c.CurriculumId == contact.CurriculumId && 
                    c.Type == contactType && 
                    c.IsPrimary &&
                    c.Id != id);
                
                foreach (var existingContact in existingPrimaryContacts)
                {
                    existingContact.IsPrimary = false;
                    _contactRepository.Update(existingContact);
                }
            }
            
            // Atualizar tipo se fornecido
            if (updateContactDto.Type.HasValue)
            {
                contact.Type = updateContactDto.Type.Value;
            }
            
            // Atualizar valor se fornecido
            if (!string.IsNullOrEmpty(updateContactDto.Value))
            {
                contact.Value = updateContactDto.Value;
            }
            
            // Atualizar indicador de primário se fornecido
            if (updateContactDto.IsPrimary.HasValue)
            {
                contact.IsPrimary = updateContactDto.IsPrimary.Value;
            }
            
            _contactRepository.Update(contact);
            await _contactRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(contact.CurriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }
            
            _logger.LogInformation("Contato atualizado: {ContactId}", contact.Id);
            
            return Ok(ApiResponse<ContactDTO>.SuccessResponse(MapToDto(contact), "Contato atualizado com sucesso"));
        }

        /// <summary>
        /// Remove um contato
        /// </summary>
        /// <param name="id">ID do contato</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Contato removido com sucesso</response>
        /// <response code="404">Contato não encontrado</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Contato não encontrado"));
            }
            
            var curriculumId = contact.CurriculumId;
            
            _contactRepository.Remove(contact);
            await _contactRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }
            
            _logger.LogInformation("Contato removido: {ContactId}", id);
            
            return Ok(ApiResponse<object>.SuccessResponse(null, "Contato removido com sucesso"));
        }

        /// <summary>
        /// Converte um modelo Contact para ContactDTO
        /// </summary>
        /// <param name="contact">Modelo Contact</param>
        /// <returns>DTO do contato</returns>
        private static ContactDTO MapToDto(Contact contact)
        {
            return new ContactDTO
            {
                Id = contact.Id,
                CurriculumId = contact.CurriculumId,
                Type = contact.Type,
                Value = contact.Value,
                IsPrimary = contact.IsPrimary
            };
        }
    }
}
