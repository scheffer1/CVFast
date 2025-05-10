using System;
using System.Threading.Tasks;
using CVFastApi.Data;
using CVFastApi.Models;
using CVFastApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVFastApi.Repositories
{
    /// <summary>
    /// Implementação do repositório de usuários
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <inheritdoc/>
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }
    }
}
