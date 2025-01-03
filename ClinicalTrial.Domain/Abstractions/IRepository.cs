namespace ClinicalTrial.Domain.Abstractions
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity);

        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetAsync(Guid id);

        Task DeleteAsync(T entity);

        Task UpdateAsync(T entity);
    }
}
