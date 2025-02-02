using CL8.DAL.Entities;
using CL8.DAL.Repositories.SpecialIntefaces;
using Microsoft.EntityFrameworkCore;

namespace CL8.DAL.Repositories
{
    public class ChatRepository(MyContext ctx) : BaseRepository<Chat>(ctx), IChatRepository
    {
        private readonly MyContext _ctx = ctx;
        public IQueryable<Chat> GetUserChats(string name) => _ctx.Chats.Include(c => c.Users).Where(c => c.Name == name);
    }
}
