using System.Linq.Expressions;

namespace CVFastServices.Repositories.Interfaces
{
    /// <summary>
    /// Interface genérica para repositórios
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Obtém uma entidade pelo seu identificador
        /// </summary>
        /// <param name="id">Identificador da entidade</param>
        /// <returns>A entidade encontrada ou null</returns>
        Task<T?> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Obtém todas as entidades
        /// </summary>
        /// <returns>Lista de entidades</returns>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// Encontra entidades que satisfazem uma condição
        /// </summary>
        /// <param name="predicate">Expressão de filtro</param>
        /// <returns>Lista de entidades que satisfazem a condição</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// Adiciona uma nova entidade
        /// </summary>
        /// <param name="entity">Entidade a ser adicionada</param>
        Task AddAsync(T entity);
        
        /// <summary>
        /// Adiciona várias entidades
        /// </summary>
        /// <param name="entities">Entidades a serem adicionadas</param>
        Task AddRangeAsync(IEnumerable<T> entities);
        
        /// <summary>
        /// Atualiza uma entidade existente
        /// </summary>
        /// <param name="entity">Entidade a ser atualizada</param>
        void Update(T entity);
        
        /// <summary>
        /// Remove uma entidade
        /// </summary>
        /// <param name="entity">Entidade a ser removida</param>
        void Remove(T entity);
        
        /// <summary>
        /// Remove várias entidades
        /// </summary>
        /// <param name="entities">Entidades a serem removidas</param>
        void RemoveRange(IEnumerable<T> entities);
        
        /// <summary>
        /// Salva as alterações no banco de dados
        /// </summary>
        Task SaveChangesAsync();
    }
}
