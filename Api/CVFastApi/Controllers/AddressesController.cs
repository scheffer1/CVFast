using CVFastApi.DTOs;
using CVFastApi.Models;
using CVFastApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CVFastApi.Controllers
{
    /// <summary>
    /// Controller para operações relacionadas a endereços
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AddressesController : ControllerBase
    {
        private readonly IRepository<Address> _addressRepository;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly ILogger<AddressesController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="addressRepository">Repositório de endereços</param>
        /// <param name="curriculumRepository">Repositório de currículos</param>
        /// <param name="logger">Logger</param>
        public AddressesController(
            IRepository<Address> addressRepository,
            ICurriculumRepository curriculumRepository,
            ILogger<AddressesController> logger)
        {
            _addressRepository = addressRepository;
            _curriculumRepository = curriculumRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os endereços
        /// </summary>
        /// <returns>Lista de endereços</returns>
        /// <response code="200">Retorna a lista de endereços</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AddressDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var addresses = await _addressRepository.GetAllAsync();
            var addressDtos = addresses.Select(MapToDto);
            
            return Ok(ApiResponse<IEnumerable<AddressDTO>>.SuccessResponse(addressDtos));
        }

        /// <summary>
        /// Obtém os endereços de um currículo específico
        /// </summary>
        /// <param name="curriculumId">ID do currículo</param>
        /// <returns>Lista de endereços do currículo</returns>
        /// <response code="200">Retorna a lista de endereços do currículo</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpGet("curriculum/{curriculumId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AddressDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCurriculumId(Guid curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }
            
            var addresses = await _addressRepository.FindAsync(a => a.CurriculumId == curriculumId);
            var addressDtos = addresses.Select(MapToDto);
            
            return Ok(ApiResponse<IEnumerable<AddressDTO>>.SuccessResponse(addressDtos));
        }

        /// <summary>
        /// Obtém um endereço pelo seu ID
        /// </summary>
        /// <param name="id">ID do endereço</param>
        /// <returns>Endereço encontrado</returns>
        /// <response code="200">Retorna o endereço</response>
        /// <response code="404">Endereço não encontrado</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<AddressDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Endereço não encontrado"));
            }
            
            return Ok(ApiResponse<AddressDTO>.SuccessResponse(MapToDto(address)));
        }

        /// <summary>
        /// Cria um novo endereço
        /// </summary>
        /// <param name="createAddressDto">Dados do endereço</param>
        /// <returns>Endereço criado</returns>
        /// <response code="201">Endereço criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Currículo não encontrado</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AddressDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateAddressDTO createAddressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            
            var curriculum = await _curriculumRepository.GetByIdAsync(createAddressDto.CurriculumId);
            if (curriculum == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Currículo não encontrado"));
            }
            
            var address = new Address
            {
                Id = Guid.NewGuid(),
                CurriculumId = createAddressDto.CurriculumId,
                Street = createAddressDto.Street,
                Number = createAddressDto.Number,
                Complement = createAddressDto.Complement,
                Neighborhood = createAddressDto.Neighborhood,
                City = createAddressDto.City,
                State = createAddressDto.State,
                Country = createAddressDto.Country,
                ZipCode = createAddressDto.ZipCode,
                Type = createAddressDto.Type
            };
            
            await _addressRepository.AddAsync(address);
            await _addressRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            curriculum.UpdatedAt = DateTime.UtcNow;
            _curriculumRepository.Update(curriculum);
            await _curriculumRepository.SaveChangesAsync();
            
            _logger.LogInformation("Endereço criado: {AddressId} para o currículo: {CurriculumId}", 
                address.Id, address.CurriculumId);
            
            return CreatedAtAction(nameof(GetById), new { id = address.Id }, 
                ApiResponse<AddressDTO>.SuccessResponse(MapToDto(address), "Endereço criado com sucesso"));
        }

        /// <summary>
        /// Atualiza um endereço existente
        /// </summary>
        /// <param name="id">ID do endereço</param>
        /// <param name="updateAddressDto">Dados para atualização</param>
        /// <returns>Endereço atualizado</returns>
        /// <response code="200">Endereço atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Endereço não encontrado</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<AddressDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressDTO updateAddressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            
            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Endereço não encontrado"));
            }
            
            // Atualizar rua se fornecida
            if (!string.IsNullOrEmpty(updateAddressDto.Street))
            {
                address.Street = updateAddressDto.Street;
            }
            
            // Atualizar número se fornecido
            if (!string.IsNullOrEmpty(updateAddressDto.Number))
            {
                address.Number = updateAddressDto.Number;
            }
            
            // Atualizar complemento se fornecido
            if (updateAddressDto.Complement != null)
            {
                address.Complement = updateAddressDto.Complement;
            }
            
            // Atualizar bairro se fornecido
            if (!string.IsNullOrEmpty(updateAddressDto.Neighborhood))
            {
                address.Neighborhood = updateAddressDto.Neighborhood;
            }
            
            // Atualizar cidade se fornecida
            if (!string.IsNullOrEmpty(updateAddressDto.City))
            {
                address.City = updateAddressDto.City;
            }
            
            // Atualizar estado se fornecido
            if (!string.IsNullOrEmpty(updateAddressDto.State))
            {
                address.State = updateAddressDto.State;
            }
            
            // Atualizar país se fornecido
            if (updateAddressDto.Country != null)
            {
                address.Country = updateAddressDto.Country;
            }
            
            // Atualizar CEP se fornecido
            if (updateAddressDto.ZipCode != null)
            {
                address.ZipCode = updateAddressDto.ZipCode;
            }
            
            // Atualizar tipo se fornecido
            if (updateAddressDto.Type.HasValue)
            {
                address.Type = updateAddressDto.Type.Value;
            }
            
            _addressRepository.Update(address);
            await _addressRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(address.CurriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }
            
            _logger.LogInformation("Endereço atualizado: {AddressId}", address.Id);
            
            return Ok(ApiResponse<AddressDTO>.SuccessResponse(MapToDto(address), "Endereço atualizado com sucesso"));
        }

        /// <summary>
        /// Remove um endereço
        /// </summary>
        /// <param name="id">ID do endereço</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Endereço removido com sucesso</response>
        /// <response code="404">Endereço não encontrado</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Endereço não encontrado"));
            }
            
            var curriculumId = address.CurriculumId;
            
            _addressRepository.Remove(address);
            await _addressRepository.SaveChangesAsync();
            
            // Atualizar a data de modificação do currículo
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum != null)
            {
                curriculum.UpdatedAt = DateTime.UtcNow;
                _curriculumRepository.Update(curriculum);
                await _curriculumRepository.SaveChangesAsync();
            }
            
            _logger.LogInformation("Endereço removido: {AddressId}", id);
            
            return Ok(ApiResponse<object>.SuccessResponse(null, "Endereço removido com sucesso"));
        }

        /// <summary>
        /// Converte um modelo Address para AddressDTO
        /// </summary>
        /// <param name="address">Modelo Address</param>
        /// <returns>DTO do endereço</returns>
        private static AddressDTO MapToDto(Address address)
        {
            return new AddressDTO
            {
                Id = address.Id,
                CurriculumId = address.CurriculumId,
                Street = address.Street,
                Number = address.Number,
                Complement = address.Complement,
                Neighborhood = address.Neighborhood,
                City = address.City,
                State = address.State,
                Country = address.Country,
                ZipCode = address.ZipCode,
                Type = address.Type
            };
        }
    }
}
