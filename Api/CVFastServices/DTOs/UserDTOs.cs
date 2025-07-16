using System.ComponentModel.DataAnnotations;

namespace CVFastServices.DTOs
{
    /// <summary>
    /// DTO para criação de um novo usuário
    /// </summary>
    public class CreateUserDTO
    {
        /// <summary>
        /// Nome do usuário
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "O email fornecido não é válido")]
        [StringLength(255, ErrorMessage = "O email deve ter no máximo 255 caracteres")]
        public string Email { get; set; } = null!;
        
        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        public string Password { get; set; } = null!;
    }
    
    /// <summary>
    /// DTO para atualização de um usuário existente
    /// </summary>
    public class UpdateUserDTO
    {
        /// <summary>
        /// Nome do usuário
        /// </summary>
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string? Name { get; set; }
        
        /// <summary>
        /// Senha atual do usuário (necessária para alterações de segurança)
        /// </summary>
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha atual deve ter entre 6 e 100 caracteres")]
        public string? CurrentPassword { get; set; }
        
        /// <summary>
        /// Nova senha do usuário
        /// </summary>
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A nova senha deve ter entre 6 e 100 caracteres")]
        public string? NewPassword { get; set; }
    }
    
    /// <summary>
    /// DTO para autenticação de usuário
    /// </summary>
    public class LoginUserDTO
    {
        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "O email fornecido não é válido")]
        public string Email { get; set; } = null!;
        
        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; } = null!;
    }
    
    /// <summary>
    /// DTO para retorno de informações do usuário
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// Identificador único do usuário
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Nome do usuário
        /// </summary>
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// Email do usuário
        /// </summary>
        public string Email { get; set; } = null!;
        
        /// <summary>
        /// Data de criação do usuário
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
