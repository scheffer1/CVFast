using System.ComponentModel.DataAnnotations;
using CVFastApi.Models;

namespace CVFastApi.DTOs
{
    /// <summary>
    /// DTO para criação de um novo contato
    /// </summary>
    public class CreateContactDTO
    {
        /// <summary>
        /// Identificador do currículo ao qual o contato pertence
        /// </summary>
        [Required(ErrorMessage = "O ID do currículo é obrigatório")]
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Tipo de contato
        /// </summary>
        [Required(ErrorMessage = "O tipo de contato é obrigatório")]
        public ContactType Type { get; set; }
        
        /// <summary>
        /// Valor do contato (número, email, URL, etc.)
        /// </summary>
        [Required(ErrorMessage = "O valor do contato é obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O valor do contato deve ter entre 2 e 255 caracteres")]
        public string Value { get; set; } = null!;
        
        /// <summary>
        /// Indica se é o contato principal deste tipo
        /// </summary>
        [Required(ErrorMessage = "A indicação de contato principal é obrigatória")]
        public bool IsPrimary { get; set; }
    }
    
    /// <summary>
    /// DTO para atualização de um contato existente
    /// </summary>
    public class UpdateContactDTO
    {
        /// <summary>
        /// Tipo de contato
        /// </summary>
        public ContactType? Type { get; set; }
        
        /// <summary>
        /// Valor do contato (número, email, URL, etc.)
        /// </summary>
        [StringLength(255, MinimumLength = 2, ErrorMessage = "O valor do contato deve ter entre 2 e 255 caracteres")]
        public string? Value { get; set; }
        
        /// <summary>
        /// Indica se é o contato principal deste tipo
        /// </summary>
        public bool? IsPrimary { get; set; }
    }
    
    /// <summary>
    /// DTO para retorno de informações de contato
    /// </summary>
    public class ContactDTO
    {
        /// <summary>
        /// Identificador único do contato
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identificador do currículo ao qual o contato pertence
        /// </summary>
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Tipo de contato
        /// </summary>
        public ContactType Type { get; set; }
        
        /// <summary>
        /// Valor do contato (número, email, URL, etc.)
        /// </summary>
        public string Value { get; set; } = null!;
        
        /// <summary>
        /// Indica se é o contato principal deste tipo
        /// </summary>
        public bool IsPrimary { get; set; }
    }
}
