using System.ComponentModel.DataAnnotations;
using CVFastServices.Models;

namespace CVFastServices.DTOs
{
    /// <summary>
    /// DTO para criação de um novo endereço
    /// </summary>
    public class CreateAddressDTO
    {
        /// <summary>
        /// Identificador do currículo ao qual o endereço pertence
        /// </summary>
        [Required(ErrorMessage = "O ID do currículo é obrigatório")]
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Rua
        /// </summary>
        [Required(ErrorMessage = "A rua é obrigatória")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "A rua deve ter entre 2 e 255 caracteres")]
        public string Street { get; set; } = null!;
        
        /// <summary>
        /// Número
        /// </summary>
        [Required(ErrorMessage = "O número é obrigatório")]
        [StringLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres")]
        public string Number { get; set; } = null!;
        
        /// <summary>
        /// Complemento
        /// </summary>
        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string? Complement { get; set; }
        
        /// <summary>
        /// Bairro
        /// </summary>
        [Required(ErrorMessage = "O bairro é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O bairro deve ter entre 2 e 100 caracteres")]
        public string Neighborhood { get; set; } = null!;
        
        /// <summary>
        /// Cidade
        /// </summary>
        [Required(ErrorMessage = "A cidade é obrigatória")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "A cidade deve ter entre 2 e 100 caracteres")]
        public string City { get; set; } = null!;
        
        /// <summary>
        /// Estado ou província
        /// </summary>
        [Required(ErrorMessage = "O estado é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O estado deve ter entre 2 e 100 caracteres")]
        public string State { get; set; } = null!;
        
        /// <summary>
        /// País
        /// </summary>
        [StringLength(100, ErrorMessage = "O país deve ter no máximo 100 caracteres")]
        public string? Country { get; set; }
        
        /// <summary>
        /// CEP ou código postal
        /// </summary>
        [StringLength(20, ErrorMessage = "O CEP deve ter no máximo 20 caracteres")]
        public string? ZipCode { get; set; }
        
        /// <summary>
        /// Tipo de endereço
        /// </summary>
        [Required(ErrorMessage = "O tipo de endereço é obrigatório")]
        public AddressType Type { get; set; }
    }
    
    /// <summary>
    /// DTO para atualização de um endereço existente
    /// </summary>
    public class UpdateAddressDTO
    {
        /// <summary>
        /// Rua
        /// </summary>
        [StringLength(255, MinimumLength = 2, ErrorMessage = "A rua deve ter entre 2 e 255 caracteres")]
        public string? Street { get; set; }
        
        /// <summary>
        /// Número
        /// </summary>
        [StringLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres")]
        public string? Number { get; set; }
        
        /// <summary>
        /// Complemento
        /// </summary>
        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string? Complement { get; set; }
        
        /// <summary>
        /// Bairro
        /// </summary>
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O bairro deve ter entre 2 e 100 caracteres")]
        public string? Neighborhood { get; set; }
        
        /// <summary>
        /// Cidade
        /// </summary>
        [StringLength(100, MinimumLength = 2, ErrorMessage = "A cidade deve ter entre 2 e 100 caracteres")]
        public string? City { get; set; }
        
        /// <summary>
        /// Estado ou província
        /// </summary>
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O estado deve ter entre 2 e 100 caracteres")]
        public string? State { get; set; }
        
        /// <summary>
        /// País
        /// </summary>
        [StringLength(100, ErrorMessage = "O país deve ter no máximo 100 caracteres")]
        public string? Country { get; set; }
        
        /// <summary>
        /// CEP ou código postal
        /// </summary>
        [StringLength(20, ErrorMessage = "O CEP deve ter no máximo 20 caracteres")]
        public string? ZipCode { get; set; }
        
        /// <summary>
        /// Tipo de endereço
        /// </summary>
        public AddressType? Type { get; set; }
    }

    /// <summary>
    /// DTO para criação de endereço dentro de um currículo completo (sem CurriculumId)
    /// </summary>
    public class CreateAddressForCurriculumDTO
    {
        /// <summary>
        /// Rua
        /// </summary>
        [Required(ErrorMessage = "A rua é obrigatória")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "A rua deve ter entre 2 e 255 caracteres")]
        public string Street { get; set; } = null!;

        /// <summary>
        /// Número
        /// </summary>
        [Required(ErrorMessage = "O número é obrigatório")]
        [StringLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres")]
        public string Number { get; set; } = null!;

        /// <summary>
        /// Complemento
        /// </summary>
        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string? Complement { get; set; }

        /// <summary>
        /// Bairro
        /// </summary>
        [Required(ErrorMessage = "O bairro é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O bairro deve ter entre 2 e 100 caracteres")]
        public string Neighborhood { get; set; } = null!;

        /// <summary>
        /// Cidade
        /// </summary>
        [Required(ErrorMessage = "A cidade é obrigatória")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "A cidade deve ter entre 2 e 100 caracteres")]
        public string City { get; set; } = null!;

        /// <summary>
        /// Estado ou província
        /// </summary>
        [Required(ErrorMessage = "O estado é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O estado deve ter entre 2 e 100 caracteres")]
        public string State { get; set; } = null!;

        /// <summary>
        /// País
        /// </summary>
        [StringLength(100, ErrorMessage = "O país deve ter no máximo 100 caracteres")]
        public string? Country { get; set; }

        /// <summary>
        /// CEP ou código postal
        /// </summary>
        [StringLength(20, ErrorMessage = "O CEP deve ter no máximo 20 caracteres")]
        public string? ZipCode { get; set; }

        /// <summary>
        /// Tipo de endereço
        /// </summary>
        [Required(ErrorMessage = "O tipo de endereço é obrigatório")]
        public AddressType Type { get; set; }
    }

    /// <summary>
    /// DTO para retorno de informações de endereço
    /// </summary>
    public class AddressDTO
    {
        /// <summary>
        /// Identificador único do endereço
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identificador do currículo ao qual o endereço pertence
        /// </summary>
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// Rua
        /// </summary>
        public string Street { get; set; } = null!;
        
        /// <summary>
        /// Número
        /// </summary>
        public string Number { get; set; } = null!;
        
        /// <summary>
        /// Complemento
        /// </summary>
        public string? Complement { get; set; }
        
        /// <summary>
        /// Bairro
        /// </summary>
        public string Neighborhood { get; set; } = null!;
        
        /// <summary>
        /// Cidade
        /// </summary>
        public string City { get; set; } = null!;
        
        /// <summary>
        /// Estado ou província
        /// </summary>
        public string State { get; set; } = null!;
        
        /// <summary>
        /// País
        /// </summary>
        public string? Country { get; set; }
        
        /// <summary>
        /// CEP ou código postal
        /// </summary>
        public string? ZipCode { get; set; }
        
        /// <summary>
        /// Tipo de endereço
        /// </summary>
        public AddressType Type { get; set; }
    }
}
