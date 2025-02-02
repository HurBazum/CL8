using CL8.BLL.Services.SpecialInterfaces;
using CL8.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CL8.BLL
{
    public static class ServiceRegistrar
    {
        public static IServiceCollection AddServices(this IServiceCollection services) => services
            .AddTransient<IUserService, UserService>()
            .AddTransient<IMessageService, MessageService>()
            .AddTransient<IChatService, ChatService>();
    }
}