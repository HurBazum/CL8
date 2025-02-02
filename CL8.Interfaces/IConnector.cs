namespace CL8.Interfaces
{
    public interface IConnector
    {
        public Task ConnectAsync(int userId, int chatId);
        public Task ConnectMessageAndChatAsync(int messageId, string chatName);
    }
}