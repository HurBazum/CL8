using CL8.BLL.Services.SpecialInterfaces;
using CL8.UI.Infrastructure.Commands;
using CL8.UI.ViewModels.Base;
using CL8.UI.ViewModels.Tools;
using System.Windows.Input;

namespace CL8.UI.ViewModels.ChatVMs
{
    public class CreateChatViewModel(IChatService chatService) : ViewModelBase
    {
        private readonly IChatService _chatService = chatService;
        private string? _chatName;
        public string? ChatName
        {
            get => _chatName;
            set => Set(ref _chatName, value);
        }

        private string? _chatDescription;
        public string? ChatDescription
        {
            get => _chatDescription;
            set => Set(ref _chatDescription, value);
        }
        private string? _error;
        public string? Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        private ICommand? _createChatCmd;
        public ICommand CreateChatCmd => _createChatCmd ?? new LambdaCommand(CreateChatCmdExecuted, CanCreateChatCmdExecute);
        private bool CanCreateChatCmdExecute(object parameter)
        {
            if(string.IsNullOrEmpty(ChatName))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private async void CreateChatCmdExecuted(object parameter)
        {
            var result = await _chatService.CreateChatAsync(ChatName, ChatDescription, App.CurrentUser.Id);

            if(result.Value == null)
            {
                Error = result.Message;
            }
            else
            {

                CancelCommandExecuted(null);
            }
        }

        private ICommand? _cancelCmd;
        public ICommand CancelCmd => _cancelCmd ?? new LambdaCommand(CancelCommandExecuted, CanCancelCommandExecute);
        private bool CanCancelCommandExecute(object parameter) => true;
        private void CancelCommandExecuted(object parameter)
        {
            App.Store.PreviousPage();
            OnPropertyChanged(nameof(ViewModelLocator.MainViewModel.ViewModel));
        }
    }
}