namespace CVFastServices.DTOs
{
    // Removido o CreateShortLinkDTO pois os links curtos agora são gerados automaticamente

    /// <summary>
    /// DTO para retorno de informações de link curto
    /// </summary>
    public class ShortLinkDTO
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
        /// URL completa do link curto
        /// </summary>
        public string FullUrl { get; set; } = null!;

        /// <summary>
        /// URL para acessar o currículo via link curto
        /// </summary>
        public string AccessUrl { get; set; } = null!;

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
    }

    /// <summary>
    /// DTO para retorno de informações de acesso a um link curto
    /// </summary>
    public class AccessLogDTO
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
    }
}
