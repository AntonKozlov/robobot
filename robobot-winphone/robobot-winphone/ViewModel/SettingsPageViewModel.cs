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
using robobot_winphone.Model;

namespace robobot_winphone.ViewModel
{
    public class SettingsPageViewModel
    {
        private Settings settings = new Settings();

        private string ip;
        private string port;

        public bool IsUseGyroYes { get; set; }
        public bool IsUseGyroNo { get; set; }
        public bool IsRotationTurnMethod { get; set; }
        public bool IsInclinationTurnMethod { get; set; }
        public ICommand IsUseGyroYesCommand { get; private set; }
        public ICommand IsUseGyroNoCommand { get; private set; }
        public ICommand IsRotationTurnMethodCommand { get; private set; }
        public ICommand IsInclinationTurnMethodCommand { get; private set; }

        public string Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
                settings.Port = value;
            }
        }
        public string IP
        {
            get
            {
                return ip;
            }
            set
            {
                ip = value;
                settings.IP = value;
            }
        }

        public SettingsPageViewModel()
        {
            IsUseGyroYes = settings.IsUseGyro;
            IsUseGyroNo = !(IsUseGyroYes);
            IsRotationTurnMethod = ((TurnMethod)settings.TurnMethod == TurnMethod.Rotation);
            IsInclinationTurnMethod = !(IsRotationTurnMethod);
            IP = settings.IP;
            Port = settings.Port;

            IsUseGyroYesCommand = new ButtonCommand(SetUseGyroDefault);
            IsUseGyroNoCommand = new ButtonCommand(SetNotUseGyroDefault);
            IsRotationTurnMethodCommand = new ButtonCommand(SetRotationTurnDefault);
            IsInclinationTurnMethodCommand = new ButtonCommand(SetInclinationTurnDefault);
        }

        private void SetUseGyroDefault(object p)
        {
            settings.IsUseGyro = true;
        }

        private void SetNotUseGyroDefault(object p)
        {
            settings.IsUseGyro = false;
        }

        private void SetRotationTurnDefault(object p)
        {
            settings.TurnMethod = TurnMethod.Rotation;
        }

        private void SetInclinationTurnDefault(object p)
        {
            settings.TurnMethod = TurnMethod.Inclination;
        }
    }
}
