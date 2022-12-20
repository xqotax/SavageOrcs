namespace SavageOrcs.UnitOfWork
{
    public interface IUnitOfWork
    {
        void SaveChanges();

        Task<int> SaveChangesAsync();
    }
}