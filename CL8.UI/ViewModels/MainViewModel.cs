using CL8.UI.Infrastructure.Stores;
using CL8.UI.ViewModels.Base;

namespace CL8.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _title = "M-Chat";
        public string Title 
        { 
            get => _title;
            set => Set(ref _title, value);
        }

        private readonly NavigationStore _nav;

        private ViewModelBase _viewModel => _nav.CurrentViewModel;
        public ViewModelBase ViewModel
        {
            get => _viewModel;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(ViewModel));
        }


        public MainViewModel()
        {
            _nav = App.Store;
            _nav.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }
    }
}