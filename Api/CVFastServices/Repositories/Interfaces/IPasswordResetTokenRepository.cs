using CVFastServices.Models;

namespace CVFastServices.Repositories.Interfaces
{
    /// <summary>
    /// Interface para repositório de tokens de recuperação de senha
    /// </summary>
    public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
    {
        /// <summary>
        /// Busca um token válido pelo valor do token
        /// </summary>
        /// <param name="token">Token a ser buscado</param>
        /// <returns>Token de recuperação se encontrado e válido</returns>
        Task<PasswordResetToken?> GetValidTokenAsync(string token);
        
        /// <summary>
        /// Invalida todos os tokens ativos de um usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>Task</returns>
        Task InvalidateUserTokensAsync(Guid userId);
        
        /// <summary>
        /// Remove tokens expirados do banco de dados
        /// </summary>
        /// <returns>Número de tokens removidos</returns>
        Task<int> CleanupExpiredTokensAsync();
    }
}
