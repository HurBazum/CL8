using CL8.DAL.Repositories;
using CL8.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using CL8.DAL.Entities;

namespace CL8.DAL.Tools
{
    public static class Registrar
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services) => services
            .AddDbContext<MyContext>(ServiceLifetime.Singleton);

        public static IServiceCollection AddRepositories(this IServiceCollection services) => services
            .AddTransient<IRepository<User>, UserRepository>()
            .AddTransient<IRepository<Message>, MessageRepository>();
    }
}