using CL8.BLL.Infrastructure;
using CL8.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace CL8.UI.Models
{
    public class ChatViewModel : ViewModelBase
    {
        public int Id { get; init; }

        private string? _name;
        public string? Name 
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string? _description;
        public string? Description 
        {
            get => _description;
            set => Set(ref _description, value);
        }
        public ObservableCollection<UserDto> Users { get; set; } = [];
        public ObservableCollection<MessageModel> Messages { get; set; } = [];
    }
}