using System;
using System.Threading.Tasks;
using CVFastApi.Data;
using CVFastApi.Models;
using CVFastApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVFastApi.Repositories
{
    /// <summary>
    /// Implementação do repositório de links curtos
    /// </summary>
    public class ShortLinkRepository : Repository<ShortLink>, IShortLinkRepository
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        public ShortLinkRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<ShortLink?> GetByHashAsync(string hash)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Hash == hash);
        }

        /// <inheritdoc/>
        public async Task<ShortLink?> GetActiveByHashAsync(string hash)
        {
            return await _dbSet
                .Include(s => s.Curriculum)
                .FirstOrDefaultAsync(s => s.Hash == hash && !s.IsRevoked);
        }

        /// <inheritdoc/>
        public async Task<bool> RevokeAsync(Guid id)
        {
            var shortLink = await _dbSet.FindAsync(id);
            if (shortLink == null)
                return false;

            shortLink.IsRevoked = true;
            shortLink.RevokedAt = DateTime.UtcNow;
            
            return true;
        }
    }
}
