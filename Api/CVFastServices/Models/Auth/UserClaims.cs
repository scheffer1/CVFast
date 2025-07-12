namespace CVFastServices.Models.Auth
{
    /// <summary>
    /// Claims do usuário para o token JWT
    /// </summary>
    public class UserClaims
    {
        /// <summary>
        /// Identificador único do usuário
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Nome do usuário
        /// </summary>
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// Email do usuário
        /// </summary>
        public string Email { get; set; } = null!;
    }
}
