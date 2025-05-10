using System;

namespace CVFastApi.Models
{
    /// <summary>
    /// Representa uma experiência profissional no currículo
    /// </summary>
    public class Experience
    {
        /// <summary>
        /// Identificador único da experiência
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identificador do currículo ao qual a experiência pertence
        /// </summary>
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Nome da empresa
        /// </summary>
        public string CompanyName { get; set; } = null!;
        
        /// <summary>
        /// Cargo/função exercida
        /// </summary>
        public string Role { get; set; } = null!;
        
        /// <summary>
        /// Descrição das atividades realizadas
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Data de início da experiência
        /// </summary>
        public DateOnly StartDate { get; set; }
        
        /// <summary>
        /// Data de término da experiência (null se for o emprego atual)
        /// </summary>
        public DateOnly? EndDate { get; set; }
        
        /// <summary>
        /// Localização da experiência (cidade, país ou remoto)
        /// </summary>
        public string? Location { get; set; }
        
        /// <summary>
        /// Currículo ao qual a experiência pertence
        /// </summary>
        public virtual Curriculum Curriculum { get; set; } = null!;
    }
}
