namespace CVFastApi.Models.Auth
{
    /// <summary>
    /// Configurações do JWT
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Chave secreta para assinatura do token
        /// </summary>
        public string SecretKey { get; set; } = null!;
        
        /// <summary>
        /// Emissor do token
        /// </summary>
        public string Issuer { get; set; } = null!;
        
        /// <summary>
        /// Audiência do token
        /// </summary>
        public string Audience { get; set; } = null!;
        
        /// <summary>
        /// Tempo de expiração do token em minutos
        /// </summary>
        public int ExpirationMinutes { get; set; }
    }
}
