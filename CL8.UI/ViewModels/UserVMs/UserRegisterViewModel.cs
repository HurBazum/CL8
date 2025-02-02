using CL8.UI.Infrastructure.Commands;
using CL8.UI.ViewModels.Base;
using CL8.UI.ViewModels.Tools;
using System.Windows.Input;
using CL8.BLL.Services.SpecialInterfaces;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace CL8.UI.ViewModels.UserVMs
{
    public partial class UserRegisterViewModel(IUserService userService) : ViewModelBase
    {
        private readonly IUserService _userService = userService;

        private string? _userEmail;

        public string? UserEmail
        {
            get => _userEmail;
            set => Set(ref _userEmail, value);
        }

        private string? _login;
        public string? Login
        {
            get => _login;
            set => Set(ref _login, value);
        }

        private string? _password;
        public string? Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        private string? _confirmedPassword;
        public string? ConfirmedPassword
        {
            get => _confirmedPassword;
            set => Set(ref _confirmedPassword, value);
        }
        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set => Set(ref _errorMessage, value);
        }

        private ICommand? _toLoginViewCmd;
        public ICommand ToLoginViewCmd => _toLoginViewCmd ?? new LambdaCommand(ToLoginViewCmdExecuted, CanToLoginViewCmdExecute);
        private bool CanToLoginViewCmdExecute(object parameter) => true;
        private void ToLoginViewCmdExecuted(object parameter)
        {
            App.Store.PreviousPage();
            OnPropertyChanged(nameof(ViewModelLocator.MainViewModel.ViewModel));
        }

        private ICommand? _registerUserCmd;
        public ICommand RegisterUserCmd => _registerUserCmd ?? new LambdaCommand(RegisterUserCmdExecuted, CanRegisterUserCmdExecute);
        private bool CanRegisterUserCmdExecute(object parameter)
        {
            if(!string.IsNullOrEmpty(UserEmail) && EmailCheckRegex().IsMatch(UserEmail))
            {
                return true;
            }
            return false;
        }
        private async void RegisterUserCmdExecuted(object parameter)
        {
            var tuple = parameter as Tuple<PasswordBox, PasswordBox>;

            var result = await _userService.TryRegisterUserAsync(Login, UserEmail, tuple.Item1.Password, tuple.Item2.Password);

            if(result.Value is null)
            {
                ErrorMessage = result.Message;
            }
            else
            {
                ToLoginViewCmdExecuted(parameter);
            }
        }


        /* atleast */
        [GeneratedRegex(@"^[^@\s]+\@[^@\s]+\.[{com|ru}]+$")]
        private static partial Regex EmailCheckRegex();
    }
}