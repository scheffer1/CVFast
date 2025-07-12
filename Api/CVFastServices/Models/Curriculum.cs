namespace CVFastServices.Models
{
    /// <summary>
    /// Representa um currículo no sistema
    /// </summary>
    public class Curriculum
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
        
        /// <summary>
        /// Usuário proprietário do currículo
        /// </summary>
        public virtual User User { get; set; } = null!;
        
        /// <summary>
        /// Experiências profissionais do currículo
        /// </summary>
        public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();
        
        /// <summary>
        /// Formações acadêmicas do currículo
        /// </summary>
        public virtual ICollection<Education> Educations { get; set; } = new List<Education>();
        
        /// <summary>
        /// Habilidades técnicas do currículo
        /// </summary>
        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

        /// <summary>
        /// Idiomas do currículo
        /// </summary>
        public virtual ICollection<Language> Languages { get; set; } = new List<Language>();

        /// <summary>
        /// Contatos do currículo
        /// </summary>
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        
        /// <summary>
        /// Endereços do currículo
        /// </summary>
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        
        /// <summary>
        /// Links curtos para compartilhamento do currículo
        /// </summary>
        public virtual ICollection<ShortLink> ShortLinks { get; set; } = new List<ShortLink>();
    }
    
    /// <summary>
    /// Status possíveis para um currículo
    /// </summary>
    public enum CurriculumStatus
    {
        /// <summary>
        /// Currículo em rascunho (não finalizado)
        /// </summary>
        Draft,
        
        /// <summary>
        /// Currículo publicado e ativo
        /// </summary>
        Active,
        
        /// <summary>
        /// Currículo oculto (não visível publicamente)
        /// </summary>
        Hidden,
        
        /// <summary>
        /// Currículo arquivado (não mais utilizado)
        /// </summary>
        Archived
    }
}
