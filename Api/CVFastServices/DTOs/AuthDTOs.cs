using System.ComponentModel.DataAnnotations;

namespace CVFastServices.DTOs
{
    /// <summary>
    /// DTO para autenticação de usuário
    /// </summary>
    public class AuthRequestDTO
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
    /// DTO para resposta de autenticação
    /// </summary>
    public class AuthResponseDTO
    {
        /// <summary>
        /// Token JWT
        /// </summary>
        public string Token { get; set; } = null!;
        
        /// <summary>
        /// Data de expiração do token
        /// </summary>
        public DateTime Expiration { get; set; }
        
        /// <summary>
        /// Informações do usuário autenticado
        /// </summary>
        public UserDTO User { get; set; } = null!;
    }
    
    /// <summary>
    /// DTO para registro de um novo usuário
    /// </summary>
    public class RegisterDTO
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
        
        /// <summary>
        /// Confirmação da senha
        /// </summary>
        [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
        [Compare("Password", ErrorMessage = "As senhas não conferem")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
