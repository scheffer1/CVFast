using CVFastServices.Data;
using CVFastServices.Models;
using CVFastServices.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVFastServices.Repositories
{
    /// <summary>
    /// Implementação do repositório de currículos
    /// </summary>
    public class CurriculumRepository : Repository<Curriculum>, ICurriculumRepository
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        public CurriculumRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Curriculum>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet.Where(c => c.UserId == userId).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Curriculum?> GetCompleteByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.Experiences)
                .Include(c => c.Educations)
                .Include(c => c.Skills)
                .Include(c => c.Languages)
                .Include(c => c.Contacts)
                .Include(c => c.Addresses)
                .Include(c => c.ShortLinks)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
