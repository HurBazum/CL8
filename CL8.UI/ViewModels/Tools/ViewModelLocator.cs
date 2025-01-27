using CL8.UI.ViewModels.UserVMs;
using Microsoft.Extensions.DependencyInjection;

namespace CL8.UI.ViewModels.Tools
{
    public class ViewModelLocator
    {
        public static MainViewModel MainViewModel => App.Host.Services.GetRequiredService<MainViewModel>();
        public static UserLoginViewModel UserLoginViewModel => App.Host.Services.GetRequiredService<UserLoginViewModel>();
        public static UserRegisterViewModel UserRegisterViewModel => App.Host.Services.GetRequiredService<UserRegisterViewModel>();
        public static UserPageViewModel UserPageViewModel => App.Host.Services.GetRequiredService<UserPageViewModel>();
    }
}