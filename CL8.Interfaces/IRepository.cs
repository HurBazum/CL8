using System.Linq.Expressions;

namespace CL8.Interfaces
{
    public interface IRepository<T> where T : class, IEntity, new()
    {
        public Task<T> AddEntityAsync(T entity);
        public Task<T> GetEntityAsync(Expression<Func<T, bool>> predicate);
        public Task<IEnumerable<T>> GetEntitiesAsync(Expression<Func<T, bool>> predicate);
        public Task<T> UpdateEntityAsync(T entity);
        public Task<T> DeleteEntityAsync(int id);
        public IQueryable<T> GetAllEntities();
    }
}