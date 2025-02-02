using CL8.BLL.Infrastructure;
using CL8.Interfaces;

namespace CL8.BLL.Services.SpecialInterfaces
{
    public interface IChatService
    {
        public Task<IResponse<ChatDto>> CreateChatAsync(string? chatName, string? chatDescription,int userId);
        public Task<IResponse<List<ChatDto>>> GetUserChatsAsync(int userId);
    }
}