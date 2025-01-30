using System.Linq.Expressions;

namespace CL8.Interfaces
{
    public interface IEntityService<T> where T : class, IEntity, new()
    {
        public Task<IResponse<T>> GetEntityAsync(Expression<Func<T, bool>> predicate);
    }
}