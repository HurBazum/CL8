using CL8.DAL.Entities;

namespace CL8.BLL.Infrastructure
{
    public static class Transformer
    {
        public static UserDto ToDto(User user) => new() 
        { 
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Password = user.Password,
            Chats = ToDto(user.Chats)
        };
        public static MessageDto ToDto(Message message) => new()
        { 
            Id = message.Id, 
            Name = message.Name, 
            Content = message.Content, 
            CreatedDate = message.CreatedDate,
            UserId = message.UserId
        };

        public static ChatDto ToDto(Chat chat) => new()
        {
            Id = chat.Id,
            Name = chat.Name,
            Description = chat.Description
        };

        public static List<ChatDto> ToDto(IEnumerable<Chat> chats) => Enumerable.Select(chats, x => ToDto(x)).ToList();
        public static IEnumerable<MessageDto> ToDto(IEnumerable<Message> messages) => Enumerable.Select(messages, x => ToDto(x));
    }
}