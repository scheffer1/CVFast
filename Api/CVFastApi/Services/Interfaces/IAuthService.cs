using CVFastApi.DTOs;
using CVFastApi.Models;
using CVFastApi.Models.Auth;

namespace CVFastApi.Services.Interfaces
{
    /// <summary>
    /// Interface para o serviço de autenticação
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Autentica um usuário
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <param name="password">Senha do usuário</param>
        /// <returns>Resposta de autenticação ou null se falhar</returns>
        Task<AuthResponseDTO?> AuthenticateAsync(string email, string password);
        
        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="registerDto">Dados de registro</param>
        /// <returns>Usuário registrado ou null se falhar</returns>
        Task<UserDTO?> RegisterAsync(RegisterDTO registerDto);
        
        /// <summary>
        /// Gera um token JWT para um usuário
        /// </summary>
        /// <param name="user">Usuário</param>
        /// <returns>Token JWT e data de expiração</returns>
        (string token, DateTime expiration) GenerateJwtToken(User user);
        
        /// <summary>
        /// Valida um token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Claims do usuário ou null se o token for inválido</returns>
        UserClaims? ValidateToken(string token);
        
        /// <summary>
        /// Gera um hash para a senha
        /// </summary>
        /// <param name="password">Senha em texto plano</param>
        /// <returns>Hash da senha</returns>
        string HashPassword(string password);
        
        /// <summary>
        /// Verifica se uma senha corresponde ao hash armazenado
        /// </summary>
        /// <param name="password">Senha em texto plano</param>
        /// <param name="passwordHash">Hash da senha armazenado</param>
        /// <returns>True se a senha corresponder ao hash, False caso contrário</returns>
        bool VerifyPassword(string password, string passwordHash);
    }
}
