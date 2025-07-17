using CVFastServices.Data;
using CVFastServices.Models;
using CVFastServices.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVFastServices.Repositories
{
    /// <summary>
    /// Repositório para tokens de recuperação de senha
    /// </summary>
    public class PasswordResetTokenRepository : Repository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        public PasswordResetTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
        {
            return await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token && 
                                        !t.IsUsed && 
                                        t.ExpiresAt > DateTime.UtcNow);
        }

        /// <inheritdoc/>
        public async Task InvalidateUserTokensAsync(Guid userId)
        {
            var activeTokens = await _context.PasswordResetTokens
                .Where(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.IsUsed = true;
                token.UsedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<int> CleanupExpiredTokensAsync()
        {
            var expiredTokens = await _context.PasswordResetTokens
                .Where(t => t.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            _context.PasswordResetTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();

            return expiredTokens.Count;
        }
    }
}
