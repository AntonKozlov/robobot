using System;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;

namespace robobot_winphone.Model.Converters
{
    public class ConnectionStatusConverter : IValueConverter
    {
        private const string Red = "Red";
        private const string Green = "Green";

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {

            if (targetType.Name == "Boolean")
            {
                return (ConvertToBoolean((ViewModel.ConnectionStatus) value));
            }

            if (targetType.Name == "Brush")
            {
                return (ConvertToBrushName((ViewModel.ConnectionStatus)value));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        private bool ConvertToBoolean(ViewModel.ConnectionStatus value)
        {
            return value == ViewModel.ConnectionStatus.Connected;
        }

        private string ConvertToBrushName(ViewModel.ConnectionStatus value)
        {
            if (value == ViewModel.ConnectionStatus.Connected)
            {
                return Green;
            }
            else
            {
                return Red;
            }
        }
    }
}
