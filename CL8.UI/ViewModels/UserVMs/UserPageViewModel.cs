using CL8.BLL.Infrastructure;
using CL8.BLL.Services.SpecialInterfaces;
using CL8.UI.Infrastructure.Commands;
using CL8.UI.ViewModels.Base;
using CL8.UI.ViewModels.Tools;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CL8.UI.ViewModels.UserVMs
{
    public class UserPageViewModel(IMessageService messageService) : ViewModelBase
    {
        private readonly IMessageService _messageService = messageService;
        private string _login = App.CurrentUser!.Name;
        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }
        private string? _message;
        public string? Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        private string? _error;
        public string? Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        public ObservableCollection<MessageDto> Messages { get; set; } = [];

        private ICommand? _exitCmd;
        public ICommand ExitCmd => _exitCmd ?? new LambdaCommand(ExitCmdExecuted, CanExitCmdExecute);
        private bool CanExitCmdExecute(object parameter) => true;
        private void ExitCmdExecuted(object parameter)
        {
            App.Store.Exit();
            OnPropertyChanged(nameof(ViewModelLocator.MainViewModel.ViewModel));
            App.CurrentUser = null;
        }
        private ICommand? _sendMessageCmd;
        public ICommand SendMessageCmd => _sendMessageCmd ?? new LambdaCommand(SendMessageCmdExecuted, CanSendMessageCmdExecute);
        private bool CanSendMessageCmdExecute(object parameter) => true;
        private async void SendMessageCmdExecuted(object parameter)
        {
            var result = await _messageService.TryToSendMessageAsync(Message, App.CurrentUser.Id);

            if(result.Value != null)
            {
                Messages.Add(result.Value);
                OnPropertyChanged(nameof(Messages));
            }
            else
            {
                Error = result.Message;
            }
        }
    }
}