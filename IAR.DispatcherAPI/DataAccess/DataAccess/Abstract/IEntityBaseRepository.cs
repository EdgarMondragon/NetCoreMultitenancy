using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Entities;


namespace DataAccess.Abstract
{
    public interface IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> GetAll();
        int Count();
        T GetSingle(int id);
        T GetSingle(Expression<Func<T, bool>> predicate);
        T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        T Add(T entity);
        T Update(T entity);
        int Delete(T entity);
        void DeleteWhere(Expression<Func<T, bool>> predicate);
        void Commit();
        IEnumerable<T> ExecWithStoreProcedure(string query);
        Task<IEnumerable<T>> ExecWithStoreProcedureAsync(string query);
        Task<T> GetAsync(int id);
        T Get(int id);
        Task<ICollection<T>> GetAllAsync();
        T Find(Expression<Func<T, bool>> match);
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        ICollection<T> FindAll(Expression<Func<T, bool>> match);
        Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match);
        Task<T> AddAsync(T t);
        IEnumerable<T> AddAll(IEnumerable<T> tList);
        Task<IEnumerable<T>> AddAllAsync(IEnumerable<T> tList);
        Task<T> UpdateAsync(T updated);
        Task<int> DeleteAsync(T t);
        Task<int> CountAsync();

    }
}
