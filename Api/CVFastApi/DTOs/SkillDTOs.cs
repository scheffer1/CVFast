using System.ComponentModel.DataAnnotations;
using CVFastApi.Models;

namespace CVFastApi.DTOs
{
    /// <summary>
    /// DTO para criação de uma nova habilidade técnica
    /// </summary>
    public class CreateSkillDTO
    {
        /// <summary>
        /// Identificador do currículo ao qual a habilidade pertence
        /// </summary>
        [Required(ErrorMessage = "O ID do currículo é obrigatório")]
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Nome da tecnologia ou habilidade
        /// </summary>
        [Required(ErrorMessage = "O nome da tecnologia é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome da tecnologia deve ter entre 2 e 100 caracteres")]
        public string TechName { get; set; } = null!;
        
        /// <summary>
        /// Nível de proficiência na habilidade
        /// </summary>
        [Required(ErrorMessage = "O nível de proficiência é obrigatório")]
        public ProficiencyLevel Proficiency { get; set; }
    }
    
    /// <summary>
    /// DTO para atualização de uma habilidade técnica existente
    /// </summary>
    public class UpdateSkillDTO
    {
        /// <summary>
        /// Nome da tecnologia ou habilidade
        /// </summary>
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome da tecnologia deve ter entre 2 e 100 caracteres")]
        public string? TechName { get; set; }
        
        /// <summary>
        /// Nível de proficiência na habilidade
        /// </summary>
        public ProficiencyLevel? Proficiency { get; set; }
    }

    /// <summary>
    /// DTO para criação de skill dentro de um currículo completo (sem CurriculumId)
    /// </summary>
    public class CreateSkillForCurriculumDTO
    {
        /// <summary>
        /// Nome da tecnologia ou habilidade
        /// </summary>
        [Required(ErrorMessage = "O nome da tecnologia é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome da tecnologia deve ter entre 2 e 100 caracteres")]
        public string TechName { get; set; } = null!;

        /// <summary>
        /// Nível de proficiência na habilidade
        /// </summary>
        [Required(ErrorMessage = "O nível de proficiência é obrigatório")]
        public ProficiencyLevel Proficiency { get; set; }
    }

    /// <summary>
    /// DTO para retorno de informações de habilidade técnica
    /// </summary>
    public class SkillDTO
    {
        /// <summary>
        /// Identificador único da habilidade
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identificador do currículo ao qual a habilidade pertence
        /// </summary>
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Nome da tecnologia ou habilidade
        /// </summary>
        public string TechName { get; set; } = null!;
        
        /// <summary>
        /// Nível de proficiência na habilidade
        /// </summary>
        public ProficiencyLevel Proficiency { get; set; }
    }
}
