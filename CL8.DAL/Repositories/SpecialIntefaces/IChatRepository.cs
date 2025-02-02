using CL8.DAL.Entities;

namespace CL8.DAL.Repositories.SpecialIntefaces
{
    public interface IChatRepository
    {
        public IQueryable<Chat> GetUserChats(string email); 
    }
}