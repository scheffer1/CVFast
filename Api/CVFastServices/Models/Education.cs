namespace CVFastServices.Models
{
    /// <summary>
    /// Representa uma formação acadêmica no currículo
    /// </summary>
    public class Education
    {
        /// <summary>
        /// Identificador único da formação
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identificador do currículo ao qual a formação pertence
        /// </summary>
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Nome da instituição de ensino
        /// </summary>
        public string Institution { get; set; } = null!;
        
        /// <summary>
        /// Grau ou título obtido
        /// </summary>
        public string Degree { get; set; } = null!;
        
        /// <summary>
        /// Área de estudo ou curso
        /// </summary>
        public string FieldOfStudy { get; set; } = null!;
        
        /// <summary>
        /// Data de início da formação
        /// </summary>
        public DateOnly StartDate { get; set; }
        
        /// <summary>
        /// Data de conclusão da formação (null se estiver em andamento)
        /// </summary>
        public DateOnly? EndDate { get; set; }
        
        /// <summary>
        /// Descrição adicional sobre a formação
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Currículo ao qual a formação pertence
        /// </summary>
        public virtual Curriculum Curriculum { get; set; } = null!;
    }
}
