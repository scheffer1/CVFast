using System.Security.Cryptography;
using System.Text;
using CVFastApi.DTOs;
using CVFastApi.Models;
using CVFastApi.Repositories.Interfaces;
using CVFastApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVFastApi.Controllers
{
    /// <summary>
    /// Controller para operações relacionadas a usuários
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="userRepository">Repositório de usuários</param>
        /// <param name="logger">Logger</param>
        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os usuários
        /// </summary>
        /// <returns>Lista de usuários</returns>
        /// <response code="200">Retorna a lista de usuários</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(MapToDto);

            return Ok(ApiResponse<IEnumerable<UserDTO>>.SuccessResponse(userDtos));
        }

        /// <summary>
        /// Obtém um usuário pelo seu ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Usuário encontrado</returns>
        /// <response code="200">Retorna o usuário</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Usuário não encontrado"));
            }

            return Ok(ApiResponse<UserDTO>.SuccessResponse(MapToDto(user)));
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="createUserDto">Dados do usuário</param>
        /// <returns>Usuário criado</returns>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="409">Email já está em uso</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            // Verificar se o email já está em uso
            if (await _userRepository.EmailExistsAsync(createUserDto.Email))
            {
                return Conflict(ApiResponse<object>.ErrorResponse("Email já está em uso"));
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                PasswordHash = HashPassword(createUserDto.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("Usuário criado: {UserId}", user.Id);

            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                ApiResponse<UserDTO>.SuccessResponse(MapToDto(user), "Usuário criado com sucesso"));
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="updateUserDto">Dados para atualização</param>
        /// <returns>Usuário atualizado</returns>
        /// <response code="200">Usuário atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDTO updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Usuário não encontrado"));
            }

            // Atualizar nome se fornecido
            if (!string.IsNullOrEmpty(updateUserDto.Name))
            {
                user.Name = updateUserDto.Name;
            }

            // Atualizar senha se fornecida
            if (!string.IsNullOrEmpty(updateUserDto.CurrentPassword) && !string.IsNullOrEmpty(updateUserDto.NewPassword))
            {
                // Verificar senha atual
                if (user.PasswordHash != HashPassword(updateUserDto.CurrentPassword))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Senha atual incorreta"));
                }

                user.PasswordHash = HashPassword(updateUserDto.NewPassword);
            }

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("Usuário atualizado: {UserId}", user.Id);

            return Ok(ApiResponse<UserDTO>.SuccessResponse(MapToDto(user), "Usuário atualizado com sucesso"));
        }

        /// <summary>
        /// Remove um usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Usuário removido com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Usuário não encontrado"));
            }

            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("Usuário removido: {UserId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Usuário removido com sucesso"));
        }

        /// <summary>
        /// Autentica um usuário
        /// </summary>
        /// <param name="loginDto">Dados de login</param>
        /// <returns>Usuário autenticado</returns>
        /// <response code="200">Autenticação bem-sucedida</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || user.PasswordHash != HashPassword(loginDto.Password))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Credenciais inválidas"));
            }

            _logger.LogInformation("Usuário autenticado: {UserId}", user.Id);

            return Ok(ApiResponse<UserDTO>.SuccessResponse(MapToDto(user), "Autenticação bem-sucedida"));
        }

        /// <summary>
        /// Converte um modelo User para UserDTO
        /// </summary>
        /// <param name="user">Modelo User</param>
        /// <returns>DTO do usuário</returns>
        private static UserDTO MapToDto(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }

        /// <summary>
        /// Gera um hash para a senha
        /// </summary>
        /// <param name="password">Senha em texto plano</param>
        /// <returns>Hash da senha</returns>
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
