using CVFastServices.Models;

namespace CVFastApi.Integration.DTOs
{
    /// <summary>
    /// DTO específico para resposta da API de integração - espelha exatamente o que é exibido nas telas
    /// </summary>
    public class CurriculumIntegrationDTO
    {
        /// <summary>
        /// Título do currículo
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Resumo/descrição do currículo
        /// </summary>
        public string? Summary { get; set; }
        /// <summary>
        /// Endereço
        /// </summary>
        public string? Address { get; set; }
        /// <summary>
        /// Experiências profissionais
        /// </summary>
        public ICollection<ExperienceIntegrationDTO> Experiences { get; set; } = new List<ExperienceIntegrationDTO>();

        /// <summary>
        /// Formações acadêmicas
        /// </summary>
        public ICollection<EducationIntegrationDTO> Educations { get; set; } = new List<EducationIntegrationDTO>();

        /// <summary>
        /// Habilidades técnicas
        /// </summary>
        public ICollection<SkillIntegrationDTO> Skills { get; set; } = new List<SkillIntegrationDTO>();

        /// <summary>
        /// Idiomas
        /// </summary>
        public ICollection<LanguageIntegrationDTO> Languages { get; set; } = new List<LanguageIntegrationDTO>();

        /// <summary>
        /// Contatos
        /// </summary>
        public ICollection<ContactIntegrationDTO> Contacts { get; set; } = new List<ContactIntegrationDTO>();
    }

    /// <summary>
    /// Experiência profissional - apenas dados visíveis ao usuário
    /// </summary>
    public class ExperienceIntegrationDTO
    {
        /// <summary>
        /// Nome da empresa
        /// </summary>
        public string CompanyName { get; set; } = null!;

        /// <summary>
        /// Cargo/função
        /// </summary>
        public string Role { get; set; } = null!;

        /// <summary>
        /// Descrição das atividades
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Data de início
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Data de término (null se atual)
        /// </summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// Localização
        /// </summary>
        public string? Location { get; set; }
    }

    /// <summary>
    /// Formação acadêmica - apenas dados visíveis ao usuário
    /// </summary>
    public class EducationIntegrationDTO
    {
        /// <summary>
        /// Instituição de ensino
        /// </summary>
        public string Institution { get; set; } = null!;

        /// <summary>
        /// Grau/título
        /// </summary>
        public string Degree { get; set; } = null!;

        /// <summary>
        /// Área de estudo
        /// </summary>
        public string FieldOfStudy { get; set; } = null!;

        /// <summary>
        /// Data de início
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Data de conclusão (null se em andamento)
        /// </summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// Descrição adicional
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// Habilidade técnica - apenas dados visíveis ao usuário
    /// </summary>
    public class SkillIntegrationDTO
    {
        /// <summary>
        /// Nome da tecnologia/habilidade
        /// </summary>
        public string TechName { get; set; } = null!;

        /// <summary>
        /// Nível de proficiência
        /// </summary>
        public  ProficiencyLevel  ProficiencyLevel { get; set; }
    }

    /// <summary>
    /// Idioma - apenas dados visíveis ao usuário
    /// </summary>
    public class LanguageIntegrationDTO
    {
        /// <summary>
        /// Nome do idioma
        /// </summary>
        public string LanguageName { get; set; } = null!;

        /// <summary>
        /// Nível de proficiência
        /// </summary>
        public LanguageProficiencyLevel Proficiency { get; set; }
    }

    /// <summary>
    /// Contato - apenas dados visíveis ao usuário
    /// </summary>
    public class ContactIntegrationDTO
    {
        /// <summary>
        /// Tipo de contato
        /// </summary>
        public ContactType Type { get; set; }

        /// <summary>
        /// Valor do contato
        /// </summary>
        public string Value { get; set; } = null!;

        /// <summary>
        /// Se é o contato principal
        /// </summary>
        public bool IsPrimary { get; set; }
    }
}
