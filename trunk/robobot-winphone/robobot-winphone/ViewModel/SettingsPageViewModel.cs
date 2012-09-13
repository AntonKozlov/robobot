using System;
using System.Windows;
using System.Windows.Input;
using robobot_winphone.Model;
using robobot_winphone.Model.DataBase;

namespace robobot_winphone.ViewModel
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private Settings settings = new Settings();

        private string ip;
        private string port;
        private string connectionName;
        private Visibility saveConnectionDialogVisibility;

        public bool IsUseGyroYes { get; set; }
        public bool IsUseGyroNo { get; set; }
        public bool IsRotationTurnMethod { get; set; }
        public bool IsInclinationTurnMethod { get; set; }
        public ICommand IsUseGyroYesCommand { get; private set; }
        public ICommand IsUseGyroNoCommand { get; private set; }
        public ICommand IsRotationTurnMethodCommand { get; private set; }
        public ICommand IsInclinationTurnMethodCommand { get; private set; }
        public ICommand SaveConnectionCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

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

        public string ConnectionName
        {
            get
            {
                return connectionName;
            }
            set
            {
                connectionName = value;
                NotifyPropertyChanged("ConnectionName");
            }
        }

        public Visibility SaveConnectionDialogVisibility
        {
            get
            {
                return saveConnectionDialogVisibility;
            }
            set
            {
                saveConnectionDialogVisibility = value;
                NotifyPropertyChanged("SaveConnectionDialogVisibility");
            }
        }

        public SettingsPageViewModel()
        {
            IsUseGyroYes = settings.IsUseGyro;
            IsUseGyroNo = !(IsUseGyroYes);
            IsRotationTurnMethod = (settings.TurnMethod == TurnMethod.Rotation);
            IsInclinationTurnMethod = !(IsRotationTurnMethod);
            IP = settings.IP;
            Port = settings.Port;
            SaveConnectionDialogVisibility = Visibility.Collapsed;

            IsUseGyroYesCommand = new ButtonCommand(SetUseGyroDefault);
            IsUseGyroNoCommand = new ButtonCommand(SetNotUseGyroDefault);
            IsRotationTurnMethodCommand = new ButtonCommand(SetRotationTurnDefault);
            IsInclinationTurnMethodCommand = new ButtonCommand(SetInclinationTurnDefault);
            SaveCommand = new ButtonCommand(Save);
            SaveConnectionCommand = new ButtonCommand(SaveConnection);
            CancelCommand = new ButtonCommand(Cancel);
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

        private void SaveConnection(object p)
        {
            SaveConnectionDialogVisibility = Visibility.Visible;
            var db = new ConnectionDataBase();
            ConnectionName = db.GetDefaultName();
        }

        private void Save(object p)
        {
            var db = new ConnectionDataBase();
            db.AddConnection(ConnectionName, IP, Convert.ToInt32(Port));
            SaveConnectionDialogVisibility = Visibility.Collapsed;
        }

        private void Cancel(object p)
        {
            SaveConnectionDialogVisibility = Visibility.Collapsed;
        }
    }
}
