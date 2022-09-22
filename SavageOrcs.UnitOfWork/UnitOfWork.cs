using SavageOrcs.DbContext;


namespace SavageOrcs.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SavageOrcsDbContext _context;
        public UnitOfWork(SavageOrcsDbContext context)
        {
            _context = context;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
