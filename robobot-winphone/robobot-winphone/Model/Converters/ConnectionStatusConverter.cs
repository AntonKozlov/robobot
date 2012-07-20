using System;
using System.Windows.Data;

namespace robobot_winphone.Model.Converters
{
    public class ConnectionStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return (ViewModel.ConnectionStatus)value == ViewModel.ConnectionStatus.Connected;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
