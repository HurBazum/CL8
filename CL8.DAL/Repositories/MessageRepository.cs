using CL8.DAL.Entities;
using CL8.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace CL8.DAL.Repositories
{
    public class MessageRepository(MyContext myContext) : IRepository<Message>
    {
        private readonly MyContext _ctx = myContext;
        public async Task<Message> AddEntityAsync(Message entity)
        {
            var entry = _ctx.Messages.Entry(entity);
            entry.State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<Message> DeleteEntityAsync(int id)
        {
            var entity = await _ctx.Messages.FirstOrDefaultAsync(x => x.Id == id);
            var entry = _ctx.Messages.Entry(entity);
            entry.State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<IEnumerable<Message>> GetEntitiesAsync(Expression<Func<Message, bool>> predicate) => await _ctx.Messages.Where(predicate).ToListAsync();

        public async Task<Message> GetEntityAsync(Expression<Func<Message, bool>> predicate) => await _ctx.Messages.FirstOrDefaultAsync(predicate);
        public async Task<Message> UpdateEntityAsync(Message entity)
        {
            var entry = _ctx.Messages.Entry(entity);
            entry.State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return entry.Entity;
        }
    }
}