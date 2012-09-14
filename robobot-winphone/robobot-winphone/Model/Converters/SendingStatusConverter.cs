using System;
using System.Windows.Data;
using robobot_winphone.ViewModel;

namespace robobot_winphone.Model.Converters
{
    public class SendingStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return (SendingStatus)value == SendingStatus.StopSending;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool) value ? SendingStatus.StopSending : SendingStatus.StartSending;
        }
    }
}
