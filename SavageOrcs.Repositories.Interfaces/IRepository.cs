using System.Linq.Expressions;

namespace SavageOrcs.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null);

        T? GetT(Expression<Func<T, bool>>? predicate, string? includeProperties = null);

        Task<T?> AddAsync(T? entity);

        Task<IEnumerable<T>?> AddRangeAsync(IEnumerable<T>? entities);

        void Add(T? entity);

        void Delete(T? entity);

        void DeleteRange(IEnumerable<T>? entities);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null);

        Task<T?> GetTAsync(Expression<Func<T, bool>>? predicate, string? includeProperties = null);
    }
}