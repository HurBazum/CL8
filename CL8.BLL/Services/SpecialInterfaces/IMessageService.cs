using CL8.BLL.Infrastructure;
using CL8.Interfaces;

namespace CL8.BLL.Services.SpecialInterfaces
{
    public interface IMessageService
    {
        public Task<IResponse<MessageDto>> TryToSendMessageAsync(string name, string? message, int userId, int chatId);
        public Task<IResponse<List<MessageDto>>> GetChatHistoryAsync(string? chatName);
    }
}
