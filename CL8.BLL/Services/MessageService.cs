using CL8.BLL.Infrastructure;
using CL8.BLL.Services.SpecialInterfaces;
using CL8.DAL.Entities;
using CL8.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CL8.BLL.Services
{
    public class MessageService(IRepository<Message> messageRepository, IConnector connector) : IMessageService
    {
        private readonly IRepository<Message> _messageRepository = messageRepository;

        private readonly IConnector _connector = connector;
        public async Task<IResponse<MessageDto>> TryToSendMessageAsync(string name, string? content, int userId, int chatId)
        {
            var response = new BaseResponse<MessageDto>();

            if(string.IsNullOrEmpty(content))
            {
                response.Message = $"Cannot send empty message!";
                return response;
            }

            try
            {
                Message message = new() { CreatedDate = DateTime.Now, UserId = userId, Content = content, Name = name, ChatId = chatId };

                var m = await _messageRepository.AddEntityAsync(message);

                response.Value = Transformer.ToDto(m);
            }
            catch
            {

            }

            return response;
        }

        public async Task<IResponse<List<MessageDto>>> GetChatHistoryAsync(string? chatName)
        {
            var response = new BaseResponse<List<MessageDto>>();

            var messages = _messageRepository.GetAllEntities().Include(m => m.Chat).Where(m => m.Chat.Name == chatName);

            //var messages = await _messageRepository.GetEntitiesAsync(m => m.ChatId == chatId);

            if(messages == null)
            {
                response.Message = $"No message here. . .";
                return response;
            }

            response.Value = [..Transformer.ToDto(messages)];

            return response;
        }
    }
}
