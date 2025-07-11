using CVFastApi.DTOs;
using CVFastApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CVFastApi.Controllers
{
    /// <summary>
    /// Controller para operações de autenticação
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="authService">Serviço de autenticação</param>
        /// <param name="logger">Logger</param>
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="registerDto">Dados de registro</param>
        /// <returns>Token JWT e informações do usuário registrado</returns>
        /// <response code="201">Usuário registrado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="409">Email já está em uso</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var authResponse = await _authService.RegisterAsync(registerDto);
            if (authResponse == null)
            {
                return Conflict(ApiResponse<object>.ErrorResponse("Email já está em uso"));
            }

            _logger.LogInformation("Usuário registrado: {UserId}", authResponse.User.Id);

            return CreatedAtAction(nameof(Login), new { email = authResponse.User.Email },
                ApiResponse<AuthResponseDTO>.SuccessResponse(authResponse, "Usuário registrado com sucesso"));
        }

        /// <summary>
        /// Autentica um usuário
        /// </summary>
        /// <param name="authRequest">Dados de autenticação</param>
        /// <returns>Token JWT e informações do usuário</returns>
        /// <response code="200">Autenticação bem-sucedida</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] AuthRequestDTO authRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var authResponse = await _authService.AuthenticateAsync(authRequest.Email, authRequest.Password);
            if (authResponse == null)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Credenciais inválidas"));
            }

            _logger.LogInformation("Usuário autenticado: {UserId}", authResponse.User.Id);

            return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(authResponse, "Autenticação bem-sucedida"));
        }
    }
}
