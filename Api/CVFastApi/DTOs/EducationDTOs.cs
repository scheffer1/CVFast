using System.ComponentModel.DataAnnotations;

namespace CVFastApi.DTOs
{
    /// <summary>
    /// DTO para criação de uma nova formação acadêmica
    /// </summary>
    public class CreateEducationDTO
    {
        /// <summary>
        /// Identificador do currículo ao qual a formação pertence
        /// </summary>
        [Required(ErrorMessage = "O ID do currículo é obrigatório")]
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Nome da instituição
        /// </summary>
        [Required(ErrorMessage = "O nome da instituição é obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O nome da instituição deve ter entre 2 e 255 caracteres")]
        public string Institution { get; set; } = null!;
        
        /// <summary>
        /// Curso ou grau
        /// </summary>
        [Required(ErrorMessage = "O curso ou grau é obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O curso ou grau deve ter entre 2 e 255 caracteres")]
        public string Degree { get; set; } = null!;
        
        /// <summary>
        /// Área de estudo
        /// </summary>
        [Required(ErrorMessage = "A área de estudo é obrigatória")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "A área de estudo deve ter entre 2 e 255 caracteres")]
        public string FieldOfStudy { get; set; } = null!;
        
        /// <summary>
        /// Data de início da formação
        /// </summary>
        [Required(ErrorMessage = "A data de início é obrigatória")]
        public DateOnly StartDate { get; set; }
        
        /// <summary>
        /// Data de conclusão da formação (null se estiver em andamento)
        /// </summary>
        public DateOnly? EndDate { get; set; }
        
        /// <summary>
        /// Descrição adicional sobre a formação
        /// </summary>
        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string? Description { get; set; }
    }
    
    /// <summary>
    /// DTO para atualização de uma formação acadêmica existente
    /// </summary>
    public class UpdateEducationDTO
    {
        /// <summary>
        /// Nome da instituição
        /// </summary>
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O nome da instituição deve ter entre 2 e 255 caracteres")]
        public string? Institution { get; set; }
        
        /// <summary>
        /// Curso ou grau
        /// </summary>
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O curso ou grau deve ter entre 2 e 255 caracteres")]
        public string? Degree { get; set; }
        
        /// <summary>
        /// Área de estudo
        /// </summary>
        [StringLength(255, MinimumLength = 2, ErrorMessage = "A área de estudo deve ter entre 2 e 255 caracteres")]
        public string? FieldOfStudy { get; set; }
        
        /// <summary>
        /// Data de início da formação
        /// </summary>
        public DateOnly? StartDate { get; set; }
        
        /// <summary>
        /// Data de conclusão da formação (null se estiver em andamento)
        /// </summary>
        public DateOnly? EndDate { get; set; }
        
        /// <summary>
        /// Descrição adicional sobre a formação
        /// </summary>
        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string? Description { get; set; }
    }
    
    /// <summary>
    /// DTO para retorno de informações de formação acadêmica
    /// </summary>
    public class EducationDTO
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
        /// Nome da instituição
        /// </summary>
        public string Institution { get; set; } = null!;
        
        /// <summary>
        /// Curso ou grau
        /// </summary>
        public string Degree { get; set; } = null!;
        
        /// <summary>
        /// Área de estudo
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
    }
}
