using CVFastApi.Models;

namespace CVFastApi.Repositories
{
    /// <summary>
    /// Interface para o repositório de idiomas
    /// </summary>
    public interface ILanguageRepository
    {
        /// <summary>
        /// Obtém todos os idiomas
        /// </summary>
        /// <returns>Lista de idiomas</returns>
        Task<IEnumerable<Language>> GetAllAsync();
        
        /// <summary>
        /// Obtém um idioma pelo seu ID
        /// </summary>
        /// <param name="id">ID do idioma</param>
        /// <returns>Idioma encontrado ou null</returns>
        Task<Language?> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Obtém todos os idiomas de um currículo específico
        /// </summary>
        /// <param name="curriculumId">ID do currículo</param>
        /// <returns>Lista de idiomas do currículo</returns>
        Task<IEnumerable<Language>> GetByCurriculumIdAsync(Guid curriculumId);
        
        /// <summary>
        /// Adiciona um novo idioma
        /// </summary>
        /// <param name="language">Idioma a ser adicionado</param>
        /// <returns>Task</returns>
        Task AddAsync(Language language);
        
        /// <summary>
        /// Atualiza um idioma existente
        /// </summary>
        /// <param name="language">Idioma a ser atualizado</param>
        void Update(Language language);
        
        /// <summary>
        /// Remove um idioma
        /// </summary>
        /// <param name="language">Idioma a ser removido</param>
        void Delete(Language language);
        
        /// <summary>
        /// Salva as alterações no banco de dados
        /// </summary>
        /// <returns>Task</returns>
        Task SaveChangesAsync();
    }
}
