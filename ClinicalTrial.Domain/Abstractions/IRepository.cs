namespace ClinicalTrial.Domain.Abstractions
{
    public interface IRepository<T> where T : class
    {
        Task<Guid> CreateAsync(T entity);

        Task<IEnumerable<T>> GetAllFilteredAsync(string? status = null, int? minParticipants = null, DateTime? startDate = null, DateTime? endDate = null);

        Task<T?> GetByIdAsync(Guid id);
    }
}
