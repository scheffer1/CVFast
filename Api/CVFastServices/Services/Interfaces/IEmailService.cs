namespace CVFastServices.Services.Interfaces
{
    /// <summary>
    /// Interface para serviços de email
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envia email de recuperação de senha
        /// </summary>
        /// <param name="email">Email do destinatário</param>
        /// <param name="resetToken">Token de recuperação</param>
        /// <param name="userName">Nome do usuário</param>
        /// <param name="emailSettingsResetTokenExpirationHours"></param>
        /// <returns>True se o email foi enviado com sucesso</returns>
        Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, string userName,
            int emailSettingsResetTokenExpirationHours);
    }
}
