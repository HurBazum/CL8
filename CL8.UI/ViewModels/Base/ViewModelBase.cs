using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CL8.UI.ViewModels.Base
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public bool Set<T>(ref T field, T value, [CallerMemberName]string propertyNAme = null)
        {
            if(Equals(field, value))
            {
                return false;
            }
            else
            {
                field = value;
                OnPropertyChanged(propertyNAme);
                return true;
            }
        }
    }
}