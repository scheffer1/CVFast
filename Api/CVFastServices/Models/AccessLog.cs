namespace CVFastServices.Models
{
    /// <summary>
    /// Representa um log de acesso a um link curto
    /// </summary>
    public class AccessLog
    {
        /// <summary>
        /// Identificador único do log de acesso
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identificador do link curto acessado
        /// </summary>
        public Guid ShortLinkId { get; set; }
        
        /// <summary>
        /// Endereço IP do visitante
        /// </summary>
        public string IP { get; set; } = null!;
        
        /// <summary>
        /// User Agent do navegador ou aplicativo
        /// </summary>
        public string? UserAgent { get; set; }
        
        /// <summary>
        /// Data e hora do acesso
        /// </summary>
        public DateTime AccessedAt { get; set; }
        
        /// <summary>
        /// Link curto que foi acessado
        /// </summary>
        public virtual ShortLink ShortLink { get; set; } = null!;
    }
}
