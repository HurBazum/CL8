using CL8.UI.ViewModels.Base;
using System.Windows.Input;
using CL8.UI.ViewModels.Tools;
using CL8.UI.Infrastructure.Commands;
using System.Windows.Controls;
using CL8.BLL.Services.SpecialInterfaces;

namespace CL8.UI.ViewModels.UserVMs
{
    public class UserLoginViewModel(IUserService userService) : ViewModelBase
    {
        private readonly IUserService _userService = userService;
        private string? _login = string.Empty;
        public string? Login
        {
            get => _login;
            set => Set(ref _login, value);
        }
        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set => Set(ref _errorMessage, value);
        }

        private ICommand? _toRegisterViewCmd;
        public ICommand ToRegisterViewCmd => _toRegisterViewCmd ?? new LambdaCommand(ToRegisterViewCmdExecuted, CanToRegisterViewCmdExecute);
        private bool CanToRegisterViewCmdExecute(object parameter) => true;
        private void ToRegisterViewCmdExecuted(object parameter)
        {
            App.Store.NextPage(ViewModelLocator.UserRegisterViewModel);
            OnPropertyChanged(nameof(ViewModelLocator.MainViewModel.ViewModel));
        }
        private ICommand? _loginUserCmd;
        public ICommand LoginUserCmd => _loginUserCmd ?? new LambdaCommand(LoginUserCmdExecuted, CanLoginUserCmdExecute);
        private bool CanLoginUserCmdExecute(object parameter)
        {
            if(string.IsNullOrEmpty(Login))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private async void LoginUserCmdExecuted(object parameter)
        {
            PasswordBox pBox = parameter as PasswordBox;

            var result = await _userService.TryLoginAsync(Login, pBox.Password);
            if(result.Value is not null)
            {
                App.CurrentUser = result.Value;
                App.Store.NextPage(ViewModelLocator.UserPageViewModel);
                OnPropertyChanged(nameof(ViewModelLocator.MainViewModel.ViewModel));
            }
            else
            {
                ErrorMessage = result.Message;
            }
        }
    }
}