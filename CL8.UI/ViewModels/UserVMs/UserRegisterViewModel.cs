using CL8.UI.Infrastructure.Commands;
using CL8.UI.ViewModels.Base;
using CL8.UI.ViewModels.Tools;
using System.Windows.Input;

namespace CL8.UI.ViewModels.UserVMs
{
    public class UserRegisterViewModel : ViewModelBase
    {
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

        private ICommand? _toLoginViewCmd;
        public ICommand ToLoginViewCmd => _toLoginViewCmd ?? new LambdaCommand(ToLoginViewCmdExecuted, CanToLoginViewCmdExecute);
        private bool CanToLoginViewCmdExecute(object parameter) => true;
        private void ToLoginViewCmdExecuted(object parameter)
        {
            App.Store.PreviousPage();
            OnPropertyChanged(nameof(ViewModelLocator.MainViewModel.ViewModel));
        }
    }
}