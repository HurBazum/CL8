using CL8.BLL.Infrastructure;
using CL8.Interfaces;

namespace CL8.BLL.Services.SpecialInterfaces
{
    public interface IMessageService
    {
        public Task<IResponse<MessageDto>> TryToSendMessageAsync(string? message, int userId);
    }
}
