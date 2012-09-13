using System.Windows;
using System.Windows.Media;

namespace robobot_winphone.Model.Helpers
{
    public class FindParentHelper
    {
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
