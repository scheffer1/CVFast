using CVFastServices.DTOs;
using CVFastServices.Models;
using CVFastServices.Repositories.Interfaces;
using CVFastServices.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace CVFastServices.Services
{
    /// <summary>
    /// Serviço para recuperação de senha
    /// </summary>
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetTokenRepository _tokenRepository;
        private readonly IEmailService _emailService;
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<PasswordResetService> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        public PasswordResetService(
            IUserRepository userRepository,
            IPasswordResetTokenRepository tokenRepository,
            IEmailService emailService,
            IOptions<EmailSettings> emailSettings,
            ILogger<PasswordResetService> logger)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ForgotPasswordResponseDTO> RequestPasswordResetAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    // Por segurança, sempre retornamos sucesso mesmo se o email não existir
                    return new ForgotPasswordResponseDTO
                    {
                        Message = "Se o email estiver cadastrado, você receberá as instruções de recuperação.",
                        EmailSent = true
                    };
                }

                // Invalidar tokens existentes do usuário
                await _tokenRepository.InvalidateUserTokensAsync(user.Id);

                // Gerar novo token
                var token = GenerateSecureToken();
                var resetToken = new PasswordResetToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = token,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(_emailSettings.ResetTokenExpirationHours),
                    IsUsed = false
                };

                await _tokenRepository.AddAsync(resetToken);
                await _tokenRepository.SaveChangesAsync();

                // Enviar email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(user.Email, token, user.Name, _emailSettings.ResetTokenExpirationHours);

                _logger.LogInformation("Solicitação de recuperação de senha para: {Email}", email);

                return new ForgotPasswordResponseDTO
                {
                    Message = "Se o email estiver cadastrado, você receberá as instruções de recuperação.",
                    EmailSent = emailSent
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar solicitação de recuperação de senha para: {Email}", email);
                return new ForgotPasswordResponseDTO
                {
                    Message = "Erro interno do servidor. Tente novamente mais tarde.",
                    EmailSent = false
                };
            }
        }

        /// <inheritdoc/>
        public async Task<ResetPasswordResponseDTO> ResetPasswordAsync(string token, string newPassword)
        {
            try
            {
                var resetToken = await _tokenRepository.GetValidTokenAsync(token);
                if (resetToken == null)
                {
                    return new ResetPasswordResponseDTO
                    {
                        Message = "Token inválido ou expirado.",
                        Success = false
                    };
                }

                // Atualizar senha do usuário
                var user = resetToken.User;
                user.PasswordHash = HashPassword(newPassword);

                _userRepository.Update(user);

                // Marcar token como usado
                resetToken.IsUsed = true;
                resetToken.UsedAt = DateTime.UtcNow;
                _tokenRepository.Update(resetToken);

                await _tokenRepository.SaveChangesAsync();

                _logger.LogInformation("Senha redefinida com sucesso para usuário: {UserId}", user.Id);

                return new ResetPasswordResponseDTO
                {
                    Message = "Senha redefinida com sucesso.",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao redefinir senha com token: {Token}", token);
                return new ResetPasswordResponseDTO
                {
                    Message = "Erro interno do servidor. Tente novamente mais tarde.",
                    Success = false
                };
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            try
            {
                var resetToken = await _tokenRepository.GetValidTokenAsync(token);
                return resetToken != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar token: {Token}", token);
                return false;
            }
        }

        /// <summary>
        /// Gera um token seguro para recuperação de senha
        /// </summary>
        /// <returns>Token seguro</returns>
        private static string GenerateSecureToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        /// <summary>
        /// Gera hash da senha
        /// </summary>
        /// <param name="password">Senha em texto plano</param>
        /// <returns>Hash da senha</returns>
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
