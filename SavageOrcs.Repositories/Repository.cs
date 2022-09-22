using System.Linq.Expressions;
using System.Linq;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.DbContext;
using Microsoft.EntityFrameworkCore;

namespace SavageOrcs.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly SavageOrcsDbContext _context;

        public Repository(SavageOrcsDbContext context)
        {
            _context = context;
        }

        public void Add(T? entity)
        {
            if (entity == null)
                return;
            _context.Set<T>().Add(entity);
        }

        public async Task<T?> AddAsync(T? entity)
        {
            if (entity == null)
                return null;
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<T>?> AddRangeAsync(IEnumerable<T>? entities)
        {
            if (entities == null)
                return null;
            await _context.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        public void Delete(T? entity)
        {
            if (entity == null)
                return;
            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T>? entities)
        {
            if (entities == null)
                return;
            _context.Set<T>().RemoveRange(entities);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties != null)
            {

            }
            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties != null)
            {

            }
            return await query.ToListAsync();
        }

        public T? GetT(Expression<Func<T, bool>>? predicate, string? includeProperties = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties != null)
            {

            }

            return query.FirstOrDefault();
        }

        public async Task<T?> GetTAsync(Expression<Func<T, bool>>? predicate, string? includeProperties = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties != null)
            {

            }

            return await query.FirstOrDefaultAsync();
        }
    }
}