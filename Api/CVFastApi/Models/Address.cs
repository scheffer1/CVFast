using System;

namespace CVFastApi.Models
{
    /// <summary>
    /// Representa um endereço no currículo
    /// </summary>
    public class Address
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
        /// Nome da rua
        /// </summary>
        public string Street { get; set; } = null!;
        
        /// <summary>
        /// Número do endereço
        /// </summary>
        public string Number { get; set; } = null!;
        
        /// <summary>
        /// Complemento do endereço (apartamento, bloco, etc.)
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
        
        /// <summary>
        /// Currículo ao qual o endereço pertence
        /// </summary>
        public virtual Curriculum Curriculum { get; set; } = null!;
    }
    
    /// <summary>
    /// Tipos de endereço disponíveis
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// Endereço residencial
        /// </summary>
        Residential,
        
        /// <summary>
        /// Endereço atual
        /// </summary>
        Current,
        
        /// <summary>
        /// Outro tipo de endereço
        /// </summary>
        Other
    }
}
