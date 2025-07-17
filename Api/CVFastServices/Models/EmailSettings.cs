namespace CVFastServices.Models
{
    /// <summary>
    /// Configurações de email SMTP
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// Host do servidor SMTP
        /// </summary>
        public string SmtpHost { get; set; } = null!;
        
        /// <summary>
        /// Porta do servidor SMTP
        /// </summary>
        public int SmtpPort { get; set; }
        
        /// <summary>
        /// Indica se deve usar SSL
        /// </summary>
        public bool EnableSsl { get; set; }
        
        /// <summary>
        /// Nome de usuário para autenticação SMTP
        /// </summary>
        public string Username { get; set; } = null!;
        
        /// <summary>
        /// Senha para autenticação SMTP
        /// </summary>
        public string Password { get; set; } = null!;
        
        /// <summary>
        /// Email do remetente
        /// </summary>
        public string FromEmail { get; set; } = null!;
        
        /// <summary>
        /// Nome do remetente
        /// </summary>
        public string FromName { get; set; } = null!;
        
        /// <summary>
        /// Tempo de expiração do token de reset em horas
        /// </summary>
        public int ResetTokenExpirationHours { get; set; }
    }
}
