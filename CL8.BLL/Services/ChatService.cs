using CL8.BLL.Services.SpecialInterfaces;
using CL8.Interfaces;
using CL8.DAL.Entities;
using CL8.BLL.Infrastructure;
using CL8.BLL.Infrastructure.CustomExceptions;
using System.IO.Pipelines;

namespace CL8.BLL.Services
{
    public class ChatService(IRepository<Chat> chatRepository, IConnector connector, IChatRelationReceiver<Chat> chatRelationReceiver) : IChatService
    {
        private readonly IRepository<Chat> _chatRepository = chatRepository;
        private readonly IConnector _connector = connector;
        private readonly IChatRelationReceiver<Chat> _chatRelationReceiver = chatRelationReceiver;

        public async Task<IResponse<ChatDto>> CreateChatAsync(string? chatName, string? chatDescription, int userId)
        {
            var response = new BaseResponse<ChatDto>();
            
            try
            {
                if(string.IsNullOrEmpty(chatName))
                {
                    throw new CustomException("Enter the chat name, plz!");
                }

                if(await _chatRepository.GetEntityAsync(c => c.Name == chatName) is not null)
                {
                    throw new CustomException("Chat with the same name already exists!");
                }

                var chat = await _chatRepository.AddEntityAsync(new() { Name = chatName, Description = chatDescription });

                await _connector.ConnectAsync(userId, chat.Id);

                response.Value = Transformer.ToDto(chat);
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<IResponse<List<ChatDto>>> GetUserChatsAsync(int userId)
        {
            var response = new BaseResponse<List<ChatDto>>();

            var chats = await _chatRelationReceiver.GetUserChatsAsync(userId);

            if(chats.Any())
            {
                response.Value = Transformer.ToDto(chats);
            }
            else
            {
                response.Message = $"This user has no chats";
            }

            return response;
        }
    }
}