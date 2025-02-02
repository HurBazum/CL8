using CL8.BLL.Infrastructure;
using CL8.BLL.Services.SpecialInterfaces;
using CL8.UI.Infrastructure.Commands;
using CL8.UI.ViewModels.Base;
using CL8.UI.ViewModels.Tools;
using CL8.UI.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CL8.UI.ViewModels.UserVMs
{
    public class UserPageViewModel(IMessageService messageService, IChatService chatService) : ViewModelBase
    {
        private readonly IMessageService _messageService = messageService;
        private readonly IChatService _chatService = chatService;
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

        private ChatViewModel? _currentChat;
        public ChatViewModel? CurrentChat
        {
            get => _currentChat;
            set
            {
                Set(ref _currentChat, value);
                Task.Run(async () => await InitialMessages(value));
            }
        }

        public ObservableCollection<MessageDto> Messages { get; set; } = [];

        public ObservableCollection<ChatViewModel> Chats { get; set; } = [.. Infrastructure.Converters.Transformer.ToModel(App.CurrentUser.Chats)];

        private async Task InitialMessages(ChatViewModel? value)
        {
            if(value == null)
            {
                return;
            }
            var result = await _messageService.GetChatHistoryAsync(value.Name);
            if(result.Value is not null)
            {
                Messages = [..result.Value];
                OnPropertyChanged(nameof(Messages));
            }
            else
            {
                Error = result.Message;
            }
        }

        #region cmds

        private ICommand? _exitCmd;
        public ICommand ExitCmd => _exitCmd ?? new LambdaCommand(ExitCmdExecuted, CanExitCmdExecute);
        private bool CanExitCmdExecute(object parameter) => true;
        private void ExitCmdExecuted(object parameter)
        {
            App.Store.Exit();
            OnPropertyChanged(nameof(ViewModelLocator.MainViewModel.ViewModel));
            App.CurrentUser = null;
        }

        private ICommand? _createChatCmd;
        public ICommand CreateChatCmd => _createChatCmd ?? new LambdaCommand(CreateChatCmdExecuted, CanCreateChatCmdExecute);
        private bool CanCreateChatCmdExecute(object parameter) => true;
        private async void CreateChatCmdExecuted(object parameter)
        {
            Random random = new Random();
            var result = await _chatService.CreateChatAsync($"{App.CurrentUser.Email}_chat{random.Next(1,8098)}[{Chats.Count}]", "", App.CurrentUser.Id);


            if(result.Value is not null)
            {
                Chats.Add(Infrastructure.Converters.Transformer.ToModel(result.Value));
                OnPropertyChanged(nameof(Chats));
            }
            else
            {
                Error = result.Message;
            }
        }

        private ICommand? _sendMessageCmd;
        public ICommand SendMessageCmd => _sendMessageCmd ?? new LambdaCommand(SendMessageCmdExecuted, CanSendMessageCmdExecute);
        private bool CanSendMessageCmdExecute(object parameter)
        {
            if(CurrentChat is null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private async void SendMessageCmdExecuted(object parameter)
        {
            var name = $"{App.CurrentUser.Email}_to_{CurrentChat.Name}_at_{DateTime.Now.ToShortTimeString()}";

            var result = await _messageService.TryToSendMessageAsync(name, Message, App.CurrentUser.Id, CurrentChat.Id);

            if(result.Value != null)
            {
                Messages.Add(result.Value);
                OnPropertyChanged(nameof(Messages));
                Message = string.Empty;
            }
            else
            {
                Error = result.Message;
            }
        }

        #endregion
    }
}