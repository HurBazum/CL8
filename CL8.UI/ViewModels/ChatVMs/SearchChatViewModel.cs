using CL8.BLL.Infrastructure;
using CL8.BLL.Services.SpecialInterfaces;
using CL8.UI.Infrastructure.Commands;
using CL8.UI.ViewModels.Base;
using CL8.UI.ViewModels.Tools;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CL8.UI.ViewModels.ChatVMs
{
    public class SearchChatViewModel(IChatService chatService) : ViewModelBase
    {
        private readonly IChatService _chatService = chatService;

        private string? _request;
        public string? Request
        {
            get => _request; 
            set => Set(ref _request, value);
        }
        private string? _error;
        public string? Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        public ObservableCollection<ChatDto>? Chats { get; set; }

        private ICommand? _searchCmd;
        public ICommand SearchCmd => _searchCmd ?? new LambdaCommand(SearchCmdExecuted, CanSearchCmdExecute);

        private bool CanSearchCmdExecute(object parameter)
        {
            if(string.IsNullOrEmpty(Request))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async void SearchCmdExecuted(object parameter)
        {
            var response = await _chatService.GetChatsByName(Request);

            if(response.Value == null)
            {
                Error = response.Message;
                Chats = [];
                OnPropertyChanged(nameof(Chats));
            }
            else
            {
                Chats = new(response.Value);
                OnPropertyChanged(nameof(Chats));
            }
        }

        private ICommand? _cancelCmd;
        public ICommand CancelCmd => _cancelCmd ?? new LambdaCommand(CancelCmdExecuted, CanCancelCmdExecute);
        private bool CanCancelCmdExecute(object parameter) => true;
        private void CancelCmdExecuted(object parameter)
        {
            App.Store.PreviousPage();
            OnPropertyChanged(nameof(ViewModelLocator.MainViewModel.ViewModel));
        }
    }
}