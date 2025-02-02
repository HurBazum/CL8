using CL8.DAL.Entities;
using CL8.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CL8.DAL.Tools
{
    public class ChatRelationReceiver(MyContext ctx) : IChatRelationReceiver<Chat>
    {
        private readonly MyContext _ctx = ctx;

        public async Task<IEnumerable<Chat>> GetUserChatsAsync(int id)
        {
            var user = await _ctx.Users.Include(u => u.Chats).FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
            {
                return [];
            }

            return user.Chats;
        }
    }
}