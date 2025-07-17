using System.ComponentModel.DataAnnotations;

namespace CVFastServices.DTOs
{
    /// <summary>
    /// DTO para solicitação de recuperação de senha
    /// </summary>
    public class ForgotPasswordRequestDTO
    {
        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string Email { get; set; } = null!;
    }

    /// <summary>
    /// DTO para redefinição de senha
    /// </summary>
    public class ResetPasswordRequestDTO
    {
        /// <summary>
        /// Token de recuperação
        /// </summary>
        [Required(ErrorMessage = "Token é obrigatório")]
        public string Token { get; set; } = null!;

        /// <summary>
        /// Nova senha
        /// </summary>
        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        public string NewPassword { get; set; } = null!;

        /// <summary>
        /// Confirmação da nova senha
        /// </summary>
        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("NewPassword", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmPassword { get; set; } = null!;
    }

    /// <summary>
    /// DTO de resposta para solicitação de recuperação
    /// </summary>
    public class ForgotPasswordResponseDTO
    {
        /// <summary>
        /// Mensagem de resposta
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// Indica se o email foi enviado com sucesso
        /// </summary>
        public bool EmailSent { get; set; }
    }

    /// <summary>
    /// DTO de resposta para redefinição de senha
    /// </summary>
    public class ResetPasswordResponseDTO
    {
        /// <summary>
        /// Mensagem de resposta
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// Indica se a senha foi redefinida com sucesso
        /// </summary>
        public bool Success { get; set; }
    }
}
