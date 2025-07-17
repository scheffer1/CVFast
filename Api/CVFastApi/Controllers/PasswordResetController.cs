using CVFastServices.DTOs;
using CVFastServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CVFastApi.Controllers
{
    /// <summary>
    /// Controller para recuperação de senha
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PasswordResetController : ControllerBase
    {
        private readonly IPasswordResetService _passwordResetService;
        private readonly ILogger<PasswordResetController> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="passwordResetService">Serviço de recuperação de senha</param>
        /// <param name="logger">Logger</param>
        public PasswordResetController(IPasswordResetService passwordResetService, ILogger<PasswordResetController> logger)
        {
            _passwordResetService = passwordResetService;
            _logger = logger;
        }

        /// <summary>
        /// Solicita recuperação de senha
        /// </summary>
        /// <param name="request">Dados da solicitação</param>
        /// <returns>Resposta da solicitação</returns>
        /// <response code="200">Solicitação processada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse<ForgotPasswordResponseDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var result = await _passwordResetService.RequestPasswordResetAsync(request.Email);

                _logger.LogInformation("Solicitação de recuperação de senha processada para: {Email}", request.Email);

                return Ok(ApiResponse<ForgotPasswordResponseDTO>.SuccessResponse(result, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar solicitação de recuperação de senha");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno do servidor"));
            }
        }

        /// <summary>
        /// Redefine a senha usando token de recuperação
        /// </summary>
        /// <param name="request">Dados da redefinição</param>
        /// <returns>Resposta da redefinição</returns>
        /// <response code="200">Senha redefinida com sucesso</response>
        /// <response code="400">Dados inválidos ou token inválido</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ApiResponse<ResetPasswordResponseDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var result = await _passwordResetService.ResetPasswordAsync(request.Token, request.NewPassword);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<ResetPasswordResponseDTO>.ErrorResponse(result.Message));
                }

                _logger.LogInformation("Senha redefinida com sucesso usando token");

                return Ok(ApiResponse<ResetPasswordResponseDTO>.SuccessResponse(result, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao redefinir senha");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno do servidor"));
            }
        }

        /// <summary>
        /// Valida um token de recuperação de senha
        /// </summary>
        /// <param name="token">Token a ser validado</param>
        /// <returns>Resultado da validação</returns>
        /// <response code="200">Token válido</response>
        /// <response code="400">Token inválido ou expirado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("validate-token/{token}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> ValidateToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Token é obrigatório"));
                }

                var isValid = await _passwordResetService.ValidateResetTokenAsync(token);

                if (!isValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Token inválido ou expirado"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Token válido"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar token");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno do servidor"));
            }
        }
    }
}
