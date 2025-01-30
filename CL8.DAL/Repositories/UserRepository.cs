using CL8.Interfaces;
using CL8.DAL.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace CL8.DAL.Repositories
{
    public class UserRepository(MyContext myContext) : IRepository<User>
    {
        private readonly MyContext _ctx = myContext;
        public IQueryable<User> GetAllEntities() => _ctx.Users.AsNoTracking().AsQueryable();
        public async Task<User> AddEntityAsync(User entity)
        {
            var entry = _ctx.Users.Entry(entity);
            entry.State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<User> DeleteEntityAsync(int id)
        {
            var entity = _ctx.Users.FirstOrDefault(x => x.Id == id);
            var entry = _ctx.Users.Entry(entity);
            entry.State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<User>> GetEntitiesAsync(Expression<Func<User, bool>> predicate) => await GetAllEntities().Where(predicate).ToListAsync();

        public async Task<User> GetEntityAsync(Expression<Func<User, bool>> predicate) => await GetAllEntities().FirstOrDefaultAsync(predicate);

        public async Task<User> UpdateEntityAsync(User entity)
        {
            var entry = _ctx.Users.Entry(entity);
            entry.State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return entry.Entity;
        }
    }
}