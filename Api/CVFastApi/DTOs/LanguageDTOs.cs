using System.ComponentModel.DataAnnotations;
using CVFastApi.Models;

namespace CVFastApi.DTOs
{
    /// <summary>
    /// DTO para criação de um novo idioma
    /// </summary>
    public class CreateLanguageDTO
    {
        /// <summary>
        /// Identificador do currículo ao qual o idioma pertence
        /// </summary>
        [Required(ErrorMessage = "O ID do currículo é obrigatório")]
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Nome do idioma
        /// </summary>
        [Required(ErrorMessage = "O nome do idioma é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do idioma deve ter entre 2 e 100 caracteres")]
        public string LanguageName { get; set; } = null!;
        
        /// <summary>
        /// Nível de proficiência no idioma
        /// </summary>
        [Required(ErrorMessage = "O nível de proficiência é obrigatório")]
        public LanguageProficiencyLevel Proficiency { get; set; }
    }
    
    /// <summary>
    /// DTO para atualização de um idioma existente
    /// </summary>
    public class UpdateLanguageDTO
    {
        /// <summary>
        /// Nome do idioma
        /// </summary>
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do idioma deve ter entre 2 e 100 caracteres")]
        public string? LanguageName { get; set; }
        
        /// <summary>
        /// Nível de proficiência no idioma
        /// </summary>
        public LanguageProficiencyLevel? Proficiency { get; set; }
    }
    
    /// <summary>
    /// DTO para criação de idioma junto com currículo completo
    /// </summary>
    public class CreateLanguageForCurriculumDTO
    {
        /// <summary>
        /// Nome do idioma
        /// </summary>
        [Required(ErrorMessage = "O nome do idioma é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do idioma deve ter entre 2 e 100 caracteres")]
        public string LanguageName { get; set; } = null!;
        
        /// <summary>
        /// Nível de proficiência no idioma
        /// </summary>
        [Required(ErrorMessage = "O nível de proficiência é obrigatório")]
        public LanguageProficiencyLevel Proficiency { get; set; }
    }
    
    /// <summary>
    /// DTO para retorno de informações de idioma
    /// </summary>
    public class LanguageDTO
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
    }
}
