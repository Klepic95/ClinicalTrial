namespace ClinicalTrial.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
