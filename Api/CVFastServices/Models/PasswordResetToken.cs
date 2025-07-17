namespace CVFastServices.Models
{
    /// <summary>
    /// Representa um token de recuperação de senha
    /// </summary>
    public class PasswordResetToken
    {
        /// <summary>
        /// Identificador único do token
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// ID do usuário que solicitou a recuperação
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Token único para validação
        /// </summary>
        public string Token { get; set; } = null!;
        
        /// <summary>
        /// Data de criação do token
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Data de expiração do token
        /// </summary>
        public DateTime ExpiresAt { get; set; }
        
        /// <summary>
        /// Indica se o token já foi usado
        /// </summary>
        public bool IsUsed { get; set; }
        
        /// <summary>
        /// Data em que o token foi usado (se aplicável)
        /// </summary>
        public DateTime? UsedAt { get; set; }
        
        /// <summary>
        /// Navegação para o usuário
        /// </summary>
        public User User { get; set; } = null!;
        
        /// <summary>
        /// Verifica se o token está válido (não expirado e não usado)
        /// </summary>
        public bool IsValid => !IsUsed && DateTime.UtcNow <= ExpiresAt;
    }
}
