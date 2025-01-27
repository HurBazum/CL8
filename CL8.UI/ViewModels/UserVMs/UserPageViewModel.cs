using CL8.UI.Infrastructure.Commands;
using CL8.UI.ViewModels.Base;
using CL8.UI.ViewModels.Tools;
using System.Windows.Input;

namespace CL8.UI.ViewModels.UserVMs
{
    public class UserPageViewModel : ViewModelBase
    {
        private string _login = App.CurrentUser.Name;
        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }

        private ICommand? _exitCmd;
        public ICommand ExitCmd => _exitCmd ?? new LambdaCommand(ExitCmdExecuted, CanExitCmdExecute);
        private bool CanExitCmdExecute(object parameter) => true;
        private void ExitCmdExecuted(object parameter)
        {
            App.Store.Exit();
            OnPropertyChanged(nameof(ViewModelLocator.MainViewModel.ViewModel));
            App.CurrentUser = null;
        }
    }
}