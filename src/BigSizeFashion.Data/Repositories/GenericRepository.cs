using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private BigSizeFashionChainContext _context;
        private DbSet<T> _dbSet = null;


        public GenericRepository(BigSizeFashionChainContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public IQueryable<T> GetAllByIQueryable()
        {
            return _dbSet;
        }

        public T GetByID(int id)
        {
            return _dbSet.Find(id);
        }

        public async Task<T> GetByIDAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Insert(T obj)
        {
            _dbSet.Add(obj);
        }

        public async Task InsertAsync(T obj)
        {
            await _dbSet.AddAsync(obj);
        }

        public void Update(T obj)
        {
            _dbSet.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }

        public async Task UpdateAsync(T obj)
        {
            _dbSet.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public void Delete(int id)
        {
            T existing = _dbSet.Find(id);
            _dbSet.Remove(existing);
        }

        public async Task DeleteAsync(int id)
        {
            T existing = await _dbSet.FindAsync(id);
            _dbSet.Remove(existing);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public T Find(Expression<Func<T, bool>> match)
        {
            return _dbSet.FirstOrDefault(match);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _dbSet.FirstOrDefaultAsync(match);
        }

        public IQueryable<T> sortAsc<TResult>(Expression<Func<T, TResult>> match, IQueryable<T> list)
        {
            return list.Select(x => x).OrderBy(match);
        }

        public async Task DeleteSpecificFieldByAsync(Expression<Func<T, bool>> prematch)
        {
            var rowDeleted = await FindByAsync(prematch);
            _dbSet.RemoveRange(rowDeleted);
        }
    }
}
