using CVFastServices.DTOs;

namespace CVFastServices.Services.Interfaces
{
    /// <summary>
    /// Interface para serviços de recuperação de senha
    /// </summary>
    public interface IPasswordResetService
    {
        /// <summary>
        /// Solicita recuperação de senha para um email
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <returns>Resposta da solicitação</returns>
        Task<ForgotPasswordResponseDTO> RequestPasswordResetAsync(string email);

        /// <summary>
        /// Redefine a senha usando um token válido
        /// </summary>
        /// <param name="token">Token de recuperação</param>
        /// <param name="newPassword">Nova senha</param>
        /// <returns>Resposta da redefinição</returns>
        Task<ResetPasswordResponseDTO> ResetPasswordAsync(string token, string newPassword);

        /// <summary>
        /// Valida se um token de recuperação é válido
        /// </summary>
        /// <param name="token">Token a ser validado</param>
        /// <returns>True se o token é válido</returns>
        Task<bool> ValidateResetTokenAsync(string token);
    }
}
