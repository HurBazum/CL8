using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace CL8.UI.Infrastructure.Converters
{
    public class PasswordConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Tuple<PasswordBox, PasswordBox> tuple = new((PasswordBox)values[0], (PasswordBox)values[1]);
            return tuple;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}