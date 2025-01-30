using CL8.BLL.Infrastructure;
using CL8.BLL.Services.SpecialInterfaces;
using CL8.DAL.Entities;
using CL8.Interfaces;

namespace CL8.BLL.Services
{
    public class MessageService(IRepository<Message> messageRepository) : IMessageService
    {
        private readonly IRepository<Message> _messageRepository = messageRepository;
        public async Task<IResponse<MessageDto>> TryToSendMessageAsync(string? content, int userId)
        {
            var response = new BaseResponse<MessageDto>();

            if(string.IsNullOrWhiteSpace(content))
            {
                response.Message = $"Cannot send empty message!";
                return response;
            }

            try
            {
                Message message = new() { CreatedDate = DateTime.Now, UserId = userId, Content = content, Name = "newMess" };

                var m = await _messageRepository.AddEntityAsync(message);

                response.Value = Transformer.ToDto(m);

            }
            catch
            {

            }

            return response;
        }
    }
}
