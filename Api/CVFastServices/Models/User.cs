namespace CVFastServices.Models
{
    /// <summary>
    /// Representa um usuário do sistema
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identificador único do usuário
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Email do usuário (usado para login)
        /// </summary>
        public string Email { get; set; } = null!;
        
        /// <summary>
        /// Nome do usuário
        /// </summary>
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// Hash da senha do usuário
        /// </summary>
        public string PasswordHash { get; set; } = null!;
        
        /// <summary>
        /// Data de criação do usuário
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Currículos do usuário
        /// </summary>
        public virtual ICollection<Curriculum> Curriculums { get; set; } = new List<Curriculum>();
    }
}
