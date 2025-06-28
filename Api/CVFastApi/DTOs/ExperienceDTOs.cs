using System.ComponentModel.DataAnnotations;

namespace CVFastApi.DTOs
{
    /// <summary>
    /// DTO para criação de uma nova experiência profissional
    /// </summary>
    public class CreateExperienceDTO
    {
        /// <summary>
        /// Identificador do currículo ao qual a experiência pertence
        /// </summary>
        [Required(ErrorMessage = "O ID do currículo é obrigatório")]
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Nome da empresa
        /// </summary>
        [Required(ErrorMessage = "O nome da empresa é obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O nome da empresa deve ter entre 2 e 255 caracteres")]
        public string CompanyName { get; set; } = null!;
        
        /// <summary>
        /// Cargo/função exercida
        /// </summary>
        [Required(ErrorMessage = "O cargo é obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O cargo deve ter entre 2 e 255 caracteres")]
        public string Role { get; set; } = null!;
        
        /// <summary>
        /// Descrição das atividades realizadas
        /// </summary>
        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string? Description { get; set; }
        
        /// <summary>
        /// Data de início da experiência
        /// </summary>
        [Required(ErrorMessage = "A data de início é obrigatória")]
        public DateOnly StartDate { get; set; }
        
        /// <summary>
        /// Data de término da experiência (null se for o emprego atual)
        /// </summary>
        public DateOnly? EndDate { get; set; }
        
        /// <summary>
        /// Localização da experiência (cidade, país ou remoto)
        /// </summary>
        [StringLength(255, ErrorMessage = "A localização deve ter no máximo 255 caracteres")]
        public string? Location { get; set; }
    }
    
    /// <summary>
    /// DTO para atualização de uma experiência profissional existente
    /// </summary>
    public class UpdateExperienceDTO
    {
        /// <summary>
        /// Nome da empresa
        /// </summary>
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O nome da empresa deve ter entre 2 e 255 caracteres")]
        public string? CompanyName { get; set; }
        
        /// <summary>
        /// Cargo/função exercida
        /// </summary>
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O cargo deve ter entre 2 e 255 caracteres")]
        public string? Role { get; set; }
        
        /// <summary>
        /// Descrição das atividades realizadas
        /// </summary>
        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string? Description { get; set; }
        
        /// <summary>
        /// Data de início da experiência
        /// </summary>
        public DateOnly? StartDate { get; set; }
        
        /// <summary>
        /// Data de término da experiência (null se for o emprego atual)
        /// </summary>
        public DateOnly? EndDate { get; set; }
        
        /// <summary>
        /// Localização da experiência (cidade, país ou remoto)
        /// </summary>
        [StringLength(255, ErrorMessage = "A localização deve ter no máximo 255 caracteres")]
        public string? Location { get; set; }
    }

    /// <summary>
    /// DTO para criação de experiência dentro de um currículo completo (sem CurriculumId)
    /// </summary>
    public class CreateExperienceForCurriculumDTO
    {
        /// <summary>
        /// Nome da empresa
        /// </summary>
        [Required(ErrorMessage = "O nome da empresa é obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O nome da empresa deve ter entre 2 e 255 caracteres")]
        public string CompanyName { get; set; } = null!;

        /// <summary>
        /// Cargo/função exercida
        /// </summary>
        [Required(ErrorMessage = "O cargo é obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O cargo deve ter entre 2 e 255 caracteres")]
        public string Role { get; set; } = null!;

        /// <summary>
        /// Descrição das atividades realizadas
        /// </summary>
        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string? Description { get; set; }

        /// <summary>
        /// Data de início da experiência
        /// </summary>
        [Required(ErrorMessage = "A data de início é obrigatória")]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Data de término da experiência (null se for o emprego atual)
        /// </summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// Localização da experiência (cidade, país ou remoto)
        /// </summary>
        [StringLength(255, ErrorMessage = "A localização deve ter no máximo 255 caracteres")]
        public string? Location { get; set; }
    }

    /// <summary>
    /// DTO para retorno de informações de experiência profissional
    /// </summary>
    public class ExperienceDTO
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
    }
}
