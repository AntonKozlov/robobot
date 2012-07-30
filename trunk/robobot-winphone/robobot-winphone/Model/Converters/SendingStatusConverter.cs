using System;
using System.Windows.Data;
using robobot_winphone.ViewModel;

namespace robobot_winphone.Model.Converters
{
    public class SendingStatusConverter : IValueConverter
    {
        private const int MaxValue = 10;
        private const int MinValue = 0;

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if ((SendingStatus)value == SendingStatus.StopSending)
            {
                return MaxValue;
            }
            else
            {
                return MinValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
