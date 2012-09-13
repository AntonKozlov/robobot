using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace robobot_winphone.View.Connections
{
    public partial class ConnectionItem : UserControl
    {
        public ConnectionItem()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            FindParentOfType<PhoneApplicationPage>(this).NavigationService.GoBack();
        }

        public static T FindParentOfType<T>(FrameworkElement element)
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;


            while (parent != null)
            {
                if (parent is T)
                    return (T)(object)parent;


                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }
            return default(T);
        }
    }
}
