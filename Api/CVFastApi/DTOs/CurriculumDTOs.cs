using System.ComponentModel.DataAnnotations;
using CVFastApi.Models;

namespace CVFastApi.DTOs
{
    /// <summary>
    /// DTO para criação de um novo currículo
    /// </summary>
    public class CreateCurriculumDTO
    {
        /// <summary>
        /// Título do currículo
        /// </summary>
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O título deve ter entre 2 e 255 caracteres")]
        public string Title { get; set; } = null!;
        
        /// <summary>
        /// Resumo/descrição do currículo
        /// </summary>
        [StringLength(2000, ErrorMessage = "O resumo deve ter no máximo 2000 caracteres")]
        public string? Summary { get; set; }
        
        /// <summary>
        /// Status do currículo
        /// </summary>
        [Required(ErrorMessage = "O status é obrigatório")]
        public CurriculumStatus Status { get; set; } = CurriculumStatus.Draft;
    }
    
    /// <summary>
    /// DTO para atualização de um currículo existente
    /// </summary>
    public class UpdateCurriculumDTO
    {
        /// <summary>
        /// Título do currículo
        /// </summary>
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O título deve ter entre 2 e 255 caracteres")]
        public string? Title { get; set; }
        
        /// <summary>
        /// Resumo/descrição do currículo
        /// </summary>
        [StringLength(2000, ErrorMessage = "O resumo deve ter no máximo 2000 caracteres")]
        public string? Summary { get; set; }
        
        /// <summary>
        /// Status do currículo
        /// </summary>
        public CurriculumStatus? Status { get; set; }
    }
    
    /// <summary>
    /// DTO para retorno de informações básicas do currículo
    /// </summary>
    public class CurriculumDTO
    {
        /// <summary>
        /// Identificador único do currículo
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identificador do usuário proprietário do currículo
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Título do currículo
        /// </summary>
        public string Title { get; set; } = null!;
        
        /// <summary>
        /// Resumo/descrição do currículo
        /// </summary>
        public string? Summary { get; set; }
        
        /// <summary>
        /// Status do currículo
        /// </summary>
        public CurriculumStatus Status { get; set; }
        
        /// <summary>
        /// Data de criação do currículo
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Data da última atualização do currículo
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO para retorno de informações básicas do currículo com shortLinks
    /// </summary>
    public class CurriculumWithShortLinksDTO : CurriculumDTO
    {
        /// <summary>
        /// Links curtos para compartilhamento do currículo
        /// </summary>
        public ICollection<ShortLinkDTO> ShortLinks { get; set; } = new List<ShortLinkDTO>();
    }

    /// <summary>
    /// DTO para criação de um currículo completo com todas as seções
    /// </summary>
    public class CreateCompleteCurriculumDTO
    {
        /// <summary>
        /// Título do currículo
        /// </summary>
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O título deve ter entre 2 e 255 caracteres")]
        public string Title { get; set; } = null!;

        /// <summary>
        /// Resumo/descrição do currículo
        /// </summary>
        [StringLength(2000, ErrorMessage = "O resumo deve ter no máximo 2000 caracteres")]
        public string? Summary { get; set; }

        /// <summary>
        /// Status do currículo
        /// </summary>
        [Required(ErrorMessage = "O status é obrigatório")]
        public CurriculumStatus Status { get; set; } = CurriculumStatus.Draft;

        /// <summary>
        /// Experiências profissionais do currículo
        /// </summary>
        public ICollection<CreateExperienceForCurriculumDTO> Experiences { get; set; } = new List<CreateExperienceForCurriculumDTO>();

        /// <summary>
        /// Formações acadêmicas do currículo
        /// </summary>
        public ICollection<CreateEducationForCurriculumDTO> Educations { get; set; } = new List<CreateEducationForCurriculumDTO>();

        /// <summary>
        /// Habilidades técnicas do currículo
        /// </summary>
        public ICollection<CreateSkillForCurriculumDTO> Skills { get; set; } = new List<CreateSkillForCurriculumDTO>();

        /// <summary>
        /// Contatos do currículo
        /// </summary>
        public ICollection<CreateContactForCurriculumDTO> Contacts { get; set; } = new List<CreateContactForCurriculumDTO>();

        /// <summary>
        /// Endereços do currículo
        /// </summary>
        public ICollection<CreateAddressForCurriculumDTO> Addresses { get; set; } = new List<CreateAddressForCurriculumDTO>();
    }

    /// <summary>
    /// DTO para retorno de informações completas do currículo
    /// </summary>
    public class CurriculumDetailDTO : CurriculumDTO
    {
        /// <summary>
        /// Experiências profissionais do currículo
        /// </summary>
        public ICollection<ExperienceDTO> Experiences { get; set; } = new List<ExperienceDTO>();

        /// <summary>
        /// Formações acadêmicas do currículo
        /// </summary>
        public ICollection<EducationDTO> Educations { get; set; } = new List<EducationDTO>();

        /// <summary>
        /// Habilidades técnicas do currículo
        /// </summary>
        public ICollection<SkillDTO> Skills { get; set; } = new List<SkillDTO>();

        /// <summary>
        /// Contatos do currículo
        /// </summary>
        public ICollection<ContactDTO> Contacts { get; set; } = new List<ContactDTO>();

        /// <summary>
        /// Endereços do currículo
        /// </summary>
        public ICollection<AddressDTO> Addresses { get; set; } = new List<AddressDTO>();

        /// <summary>
        /// Links curtos para compartilhamento do currículo
        /// </summary>
        public ICollection<ShortLinkDTO> ShortLinks { get; set; } = new List<ShortLinkDTO>();
    }
}
