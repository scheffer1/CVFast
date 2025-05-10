using System;

namespace CVFastApi.Models
{
    /// <summary>
    /// Representa um link curto para compartilhamento de currículo
    /// </summary>
    public class ShortLink
    {
        /// <summary>
        /// Identificador único do link curto
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identificador do currículo ao qual o link pertence
        /// </summary>
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Hash ou código do link curto
        /// </summary>
        public string Hash { get; set; } = null!;
        
        /// <summary>
        /// Indica se o link foi revogado
        /// </summary>
        public bool IsRevoked { get; set; }
        
        /// <summary>
        /// Data de criação do link
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Data em que o link foi revogado (se aplicável)
        /// </summary>
        public DateTime? RevokedAt { get; set; }
        
        /// <summary>
        /// Currículo ao qual o link pertence
        /// </summary>
        public virtual Curriculum Curriculum { get; set; } = null!;
        
        /// <summary>
        /// Logs de acesso a este link
        /// </summary>
        public virtual ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();
    }
}
