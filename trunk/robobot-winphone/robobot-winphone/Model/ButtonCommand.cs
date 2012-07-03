using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace robobot_winphone.Model
{
    public class ButtonCommand : ICommand
    {
        private Action<object> executeAction;

        public event EventHandler CanExecuteChanged;

        public ButtonCommand(Action<object> executeAction)
        {
            this.executeAction = executeAction;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.executeAction(parameter);
        }
    }
}
