using CVFastApi.Data;
using CVFastApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CVFastApi.Repositories
{
    /// <summary>
    /// Implementação do repositório de idiomas
    /// </summary>
    public class LanguageRepository : ILanguageRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        public LanguageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Language>> GetAllAsync()
        {
            return await _context.Languages
                .Include(l => l.Curriculum)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Language?> GetByIdAsync(Guid id)
        {
            return await _context.Languages
                .Include(l => l.Curriculum)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Language>> GetByCurriculumIdAsync(Guid curriculumId)
        {
            return await _context.Languages
                .Where(l => l.CurriculumId == curriculumId)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task AddAsync(Language language)
        {
            await _context.Languages.AddAsync(language);
        }

        /// <inheritdoc/>
        public void Update(Language language)
        {
            _context.Languages.Update(language);
        }

        /// <inheritdoc/>
        public void Delete(Language language)
        {
            _context.Languages.Remove(language);
        }

        /// <inheritdoc/>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
