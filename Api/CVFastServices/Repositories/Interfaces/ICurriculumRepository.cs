using CVFastServices.Models;

namespace CVFastServices.Repositories.Interfaces
{
    /// <summary>
    /// Interface para o repositório de currículos
    /// </summary>
    public interface ICurriculumRepository : IRepository<Curriculum>
    {
        /// <summary>
        /// Obtém todos os currículos de um usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>Lista de currículos do usuário</returns>
        Task<IEnumerable<Curriculum>> GetByUserIdAsync(Guid userId);
        
        /// <summary>
        /// Obtém um currículo com todas as suas entidades relacionadas
        /// </summary>
        /// <param name="id">ID do currículo</param>
        /// <returns>O currículo completo ou null</returns>
        Task<Curriculum?> GetCompleteByIdAsync(Guid id);
    }
}
