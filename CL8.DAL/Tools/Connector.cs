using CL8.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CL8.DAL.Tools
{
    public class Connector(MyContext ctx) : IConnector
    {
        private readonly MyContext _ctx = ctx;

        public async Task ConnectAsync(int userId, int chatId)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var chat = await _ctx.Chats.FirstOrDefaultAsync(c => c.Id == chatId);

            chat.Users.Add(user);
            await _ctx.SaveChangesAsync();
        }

        public async Task ConnectMessageAndChatAsync(int messageId, string chatName)
        {
            var message = await _ctx.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
            var chat = await _ctx.Chats.FirstOrDefaultAsync(c => c.Name == chatName);

            message.ChatId = chat.Id;
            message.Chat = chat;
            await _ctx.SaveChangesAsync();
        }
    }
}