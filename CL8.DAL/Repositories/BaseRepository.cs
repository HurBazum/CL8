using CL8.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CL8.DAL.Repositories
{
    public class BaseRepository<T>(MyContext ctx) : IRepository<T> where T : class, IEntity, new()
    {
        private readonly MyContext _ctx = ctx;

        public async Task<T> AddEntityAsync(T entity)
        {
            var entry = _ctx.Set<T>().Entry(entity);
            entry.State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<T> DeleteEntityAsync(int id)
        {
            var entity = await _ctx.Set<T>().FirstOrDefaultAsync(t => t.Id == id);
            var entry = _ctx.Set<T>().Entry(entity);
            entry.State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return entity;
        }

        public IQueryable<T> GetAllEntities() => _ctx.Set<T>().AsQueryable();

        public async Task<IEnumerable<T>> GetEntitiesAsync(Expression<Func<T, bool>> predicate) => await GetAllEntities().AsNoTracking().Where(predicate).ToListAsync();

        public async Task<T> GetEntityAsync(Expression<Func<T, bool>> predicate) => await _ctx.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate);

        public async Task<T> UpdateEntityAsync(T entity)
        {
            var entry = _ctx.Set<T>().Entry(entity);
            entry.State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return entry.Entity;
        }
    }
}
