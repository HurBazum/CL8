using CL8.DAL.Entities;

namespace CL8.BLL.Infrastructure
{
    public static class Transformer
    {
        public static UserDto ToDto(User user) => new() { Id = user.Id, Name = user.Name, Password = user.Password };
        public static MessageDto ToDto(Message message) => new() { Id = message.Id, Name = message.Name, Content = message.Content, CreatedDate = message.CreatedDate, UserId = message.UserId };
    }
}