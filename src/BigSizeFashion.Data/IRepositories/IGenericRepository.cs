using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Data.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAllByIQueryable();
        T GetByID(int id);
        Task<T> GetByIDAsync(int id);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        void Insert(T obj);
        Task InsertAsync(T obj);
        void Update(T obj);
        Task UpdateAsync(T obj);
        void Delete(int id);
        Task DeleteAsync(int id);
        void Save();
        Task SaveAsync();
        T Find(Expression<Func<T, bool>> match);
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        IQueryable<T> sortAsc<TResult>(Expression<Func<T, TResult>> match, IQueryable<T> list);
        Task DeleteSpecificFieldByAsync(Expression<Func<T, bool>> prematch);
    }
}
