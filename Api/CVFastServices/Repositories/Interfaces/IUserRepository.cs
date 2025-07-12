using CVFastServices.Models;

namespace CVFastServices.Repositories.Interfaces
{
    /// <summary>
    /// Interface para o repositório de usuários
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Obtém um usuário pelo seu email
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <returns>O usuário encontrado ou null</returns>
        Task<User?> GetByEmailAsync(string email);
        
        /// <summary>
        /// Verifica se um email já está em uso
        /// </summary>
        /// <param name="email">Email a ser verificado</param>
        /// <returns>True se o email já estiver em uso, False caso contrário</returns>
        Task<bool> EmailExistsAsync(string email);
    }
}
