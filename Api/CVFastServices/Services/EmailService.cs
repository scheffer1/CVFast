using CVFastServices.Models;
using CVFastServices.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CVFastServices.Services
{
    /// <summary>
    /// Serviço para envio de emails
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="emailSettings">Configurações de email</param>
        /// <param name="logger">Logger</param>
        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, string userName,
            int emailSettingsResetTokenExpirationHours)
        {
            try
            {
                var resetUrl = $"https://cvfast.com.br/reset-password?token={resetToken}";
                
                var subject = "Recuperação de Senha - CVFast";
                var body = GeneratePasswordResetEmailBody(userName, resetUrl, emailSettingsResetTokenExpirationHours);

                using var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
                {
                    EnableSsl = _emailSettings.EnableSsl,
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
                
                _logger.LogInformation("Email de recuperação de senha enviado para: {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email de recuperação de senha para: {Email}", email);
                return false;
            }
        }

        /// <summary>
        /// Gera o corpo do email de recuperação de senha
        /// </summary>
        /// <param name="userName">Nome do usuário</param>
        /// <param name="resetUrl">URL de recuperação</param>
        /// <param name="emailSettingsResetTokenExpirationHours"></param>
        /// <returns>Corpo do email em HTML</returns>
        private static string GeneratePasswordResetEmailBody(string userName, string resetUrl,
            int emailSettingsResetTokenExpirationHours)
        {
            var stringHoras = emailSettingsResetTokenExpirationHours == 1 ? "1 hora" : $"{emailSettingsResetTokenExpirationHours} horas";
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Recuperação de Senha - CVFast</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2563eb; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #2563eb; color: white !important; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .button:visited {{ color: white !important; }}
        .button:hover {{ color: white !important; background-color: #1d4ed8; }}
        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>CVFast</h1>
            <h2>Recuperação de Senha</h2>
        </div>
        <div class='content'>
            <p>Olá, <strong>{userName}</strong>!</p>
            <p>Recebemos uma solicitação para redefinir a senha da sua conta no CVFast.</p>
            <p>Para criar uma nova senha, clique no botão abaixo:</p>
            <p style='text-align: center;'>
                <a href='{resetUrl}' class='button' style='display: inline-block; padding: 12px 24px; background-color: #2563eb; color: white !important; text-decoration: none; border-radius: 5px; margin: 20px 0;'>Redefinir Senha</a>
            </p>
            <p>Se você não conseguir clicar no botão, copie e cole o link abaixo no seu navegador:</p>
            <p style='word-break: break-all; background-color: #e5e7eb; padding: 10px; border-radius: 5px;'>{resetUrl}</p>
            <p><strong>Importante:</strong> Este link é válido por apenas {stringHoras} por motivos de segurança.</p>
            <p>Se você não solicitou esta recuperação de senha, pode ignorar este email com segurança.</p>
        </div>
        <div class='footer'>
            <p>Este é um email automático, não responda.</p>
            <p>&copy; 2025 CVFast - Centralizador de Currículos</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
