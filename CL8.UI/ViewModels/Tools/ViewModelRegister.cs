using CL8.UI.ViewModels.UserVMs;
using Microsoft.Extensions.DependencyInjection;

namespace CL8.UI.ViewModels.Tools
{
    public static class ViewModelRegister
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) => services
            .AddTransient<MainViewModel>()
            .AddTransient<UserLoginViewModel>()
            .AddTransient<UserRegisterViewModel>()
            .AddTransient<UserPageViewModel>();
    }
}