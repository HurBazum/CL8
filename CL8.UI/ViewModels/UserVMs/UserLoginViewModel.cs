using CL8.UI.ViewModels.Base;
using System.Windows.Input;
using CL8.UI.ViewModels.Tools;
using CL8.UI.Infrastructure.Commands;
using CL8.Interfaces;
using CL8.DAL.Entities;

namespace CL8.UI.ViewModels.UserVMs
{
    public class UserLoginViewModel(IRepository<User> repository) : ViewModelBase
    {
        private readonly IRepository<User> _userRepository = repository;
        private string? _login = string.Empty;
        public string? Login
        {
            get => _login;
            set => Set(ref _login, value);
        }
        private string? _password = string.Empty;
        public string? Password
        {
            get => _password;
            set => Set(ref _password, value);
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
            if(string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password))
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
            //User user = new() { Name = Login, Password = Password };
            //App.CurrentUser = user;
            //App.Store.NextPage(ViewModelLocator.UserPageViewModel);
            //OnPropertyChanged(nameof(ViewModelLocator.MainViewModel.ViewModel));
            var user = await _userRepository.GetEntityAsync(u => u.Name == Login);
            if(user != null)
            {
                var confirmed = Equals(user.Password, Password);
                if(confirmed)
                {
                    App.CurrentUser = user;
                    App.Store.NextPage(ViewModelLocator.UserPageViewModel);
                    OnPropertyChanged(nameof(ViewModelLocator.MainViewModel.ViewModel));
                }
                else
                {
                    ErrorMessage = $"Неверный пароль!";
                }
            }
            else
            {
                ErrorMessage = $"Пользователя с таким логином не зарегистрировано!";
            }
        }
    }
}