using CL8.Interfaces;
using CL8.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CL8.DAL.Tools
{
    public class UserRelationReceiver(MyContext ctx) : IUserRelationReceiver<User>
    {
        private readonly MyContext _ctx = ctx;
        public async Task<User> GetOneAsync(int id) => await _ctx.Users.Include(u => u.Chats).ThenInclude(u => u.Messages).FirstOrDefaultAsync(u => u.Id == id);

        public async Task<IEnumerable<User>> GetChatsMembersAsync(int id)
        {
            var t = await _ctx.Chats.Include(u => u.Users).FirstOrDefaultAsync(c => c.Id == id);

            if(t == null)
            {
                return [];
            }

            return t.Users;
        }
    }
}