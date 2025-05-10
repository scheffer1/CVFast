using System;

namespace CVFastApi.Models
{
    /// <summary>
    /// Representa uma habilidade técnica no currículo
    /// </summary>
    public class Skill
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
        
        /// <summary>
        /// Currículo ao qual a habilidade pertence
        /// </summary>
        public virtual Curriculum Curriculum { get; set; } = null!;
    }
    
    /// <summary>
    /// Níveis de proficiência para habilidades
    /// </summary>
    public enum ProficiencyLevel
    {
        /// <summary>
        /// Nível básico
        /// </summary>
        Basic,
        
        /// <summary>
        /// Nível intermediário
        /// </summary>
        Intermediate,
        
        /// <summary>
        /// Nível avançado
        /// </summary>
        Advanced,
        
        /// <summary>
        /// Nível especialista
        /// </summary>
        Expert
    }
}
