namespace CVFastServices.Models
{
    /// <summary>
    /// Representa um contato no currículo
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Identificador único do contato
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identificador do currículo ao qual o contato pertence
        /// </summary>
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Tipo de contato
        /// </summary>
        public ContactType Type { get; set; }
        
        /// <summary>
        /// Valor do contato (número, email, URL, etc.)
        /// </summary>
        public string Value { get; set; } = null!;
        
        /// <summary>
        /// Indica se este é o contato principal deste tipo
        /// </summary>
        public bool IsPrimary { get; set; }
        
        /// <summary>
        /// Currículo ao qual o contato pertence
        /// </summary>
        public virtual Curriculum Curriculum { get; set; } = null!;
    }
    
    /// <summary>
    /// Tipos de contato disponíveis
    /// </summary>
    public enum ContactType
    {
        /// <summary>
        /// Endereço de email
        /// </summary>
        Email,
        
        /// <summary>
        /// Número de telefone
        /// </summary>
        Phone,
        
        /// <summary>
        /// Perfil do LinkedIn
        /// </summary>
        LinkedIn,
        
        /// <summary>
        /// Perfil do GitHub
        /// </summary>
        GitHub,
        
        /// <summary>
        /// Site pessoal ou portfólio
        /// </summary>
        Website,
        
        /// <summary>
        /// Número de WhatsApp
        /// </summary>
        WhatsApp
    }
}
