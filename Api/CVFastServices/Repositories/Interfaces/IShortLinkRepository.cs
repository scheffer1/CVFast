using CVFastServices.Models;

namespace CVFastServices.Repositories.Interfaces
{
    /// <summary>
    /// Interface para o repositório de links curtos
    /// </summary>
    public interface IShortLinkRepository : IRepository<ShortLink>
    {
        /// <summary>
        /// Obtém um link curto pelo seu hash
        /// </summary>
        /// <param name="hash">Hash do link curto</param>
        /// <returns>O link curto encontrado ou null</returns>
        Task<ShortLink?> GetByHashAsync(string hash);
        
        /// <summary>
        /// Obtém um link curto ativo pelo seu hash
        /// </summary>
        /// <param name="hash">Hash do link curto</param>
        /// <returns>O link curto ativo encontrado ou null</returns>
        Task<ShortLink?> GetActiveByHashAsync(string hash);
        
        /// <summary>
        /// Revoga um link curto
        /// </summary>
        /// <param name="id">ID do link curto</param>
        /// <returns>True se o link foi revogado com sucesso, False caso contrário</returns>
        Task<bool> RevokeAsync(Guid id);
    }
}
