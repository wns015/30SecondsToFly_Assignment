using System.Linq.Expressions;
using Common.Contexts.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseModel
    {
        DbContext GetContext();
    
        void Add(TEntity entity, string editor = null);
    
        void AddRange(IList<TEntity> entities, string editor = null);
    
        void Update(TEntity entity, string editor = null);
    
        void Update(TEntity entity, params Expression<Func<TEntity, object>>[] @orders);
    
        void Delete(TEntity entity);
    
        void Delete(Expression<Func<TEntity, bool>> @exp);
    
        TEntity Find(Expression<Func<TEntity, bool>> @exp);
    
        IQueryable<TEntity> FindByDescending<T>(Expression<Func<TEntity, bool>> @exp, Expression<Func<TEntity, T>> @order);
    
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> @exp);
    
        IQueryable<TEntity> Table { get; }
    
        bool SaveChanges();
    
        Task<bool> SaveChangesAsync();
    
        void Dispose();
    }
    
}
