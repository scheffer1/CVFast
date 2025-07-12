namespace CVFastServices.Models
{
    /// <summary>
    /// Representa um idioma no currículo
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Identificador único do idioma
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identificador do currículo ao qual o idioma pertence
        /// </summary>
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Nome do idioma
        /// </summary>
        public string LanguageName { get; set; } = null!;
        
        /// <summary>
        /// Nível de proficiência no idioma
        /// </summary>
        public LanguageProficiencyLevel Proficiency { get; set; }
        
        /// <summary>
        /// Currículo ao qual o idioma pertence
        /// </summary>
        public virtual Curriculum Curriculum { get; set; } = null!;
    }
    
    /// <summary>
    /// Níveis de proficiência em idiomas
    /// </summary>
    public enum LanguageProficiencyLevel
    {
        /// <summary>
        /// Iniciante
        /// </summary>
        Beginner = 1,
        
        /// <summary>
        /// Intermediário
        /// </summary>
        Intermediate = 2,
        
        /// <summary>
        /// Avançado
        /// </summary>
        Advanced = 3,
        
        /// <summary>
        /// Fluente
        /// </summary>
        Fluent = 4,
        
        /// <summary>
        /// Nativo
        /// </summary>
        Native = 5
    }
}
