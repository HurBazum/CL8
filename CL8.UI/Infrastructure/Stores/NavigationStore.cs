using CL8.UI.ViewModels.Base;
using CL8.UI.ViewModels.Tools;

namespace CL8.UI.Infrastructure.Stores
{
    public class NavigationStore
    {
        public List<ViewModelBase> ViewModels { get; set; } = [];
        public ViewModelBase CurrentViewModel => ViewModels.Last();
        public void NextPage(ViewModelBase viewModel)
        {
            ViewModels.Add(viewModel);
            OnCurrentViewModelChanged();
        }
        public void PreviousPage()
        {
            ViewModels.Remove(CurrentViewModel);
            OnCurrentViewModelChanged();
        }
        public void Exit()
        {
            ViewModels = [ViewModels[0]];
            OnCurrentViewModelChanged();
        }

        public event Action? CurrentViewModelChanged;

        private void OnCurrentViewModelChanged() => CurrentViewModelChanged?.Invoke();


        public NavigationStore()
        {
            ViewModels.Add(ViewModelLocator.UserLoginViewModel);
        }
    }
}