using System;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Collections.Generic;
using System.Device.Location;


namespace robobot_winphone.Model
{
    public enum TurnMethod : int
    {
        Rotation = 0,
        Inclination = 1
    }

    public class Settings
    {
        IsolatedStorageSettings settings;

        // The isolated storage key names of our settings
        const string IsUseGyroKeyName = "IsUseGyro";
        const string TurnMethodKeyName = "TurnMethod";
        const string PortKeyName = "Port";
        const string IPKeyName = "IP";

        // The default value of our settings
        const bool IsUseGyroDefault = true;
        const int TurnMethodDefault = (int)Model.TurnMethod.Inclination;
        const string PortDefault = "43214";
        const string IPDefault = "192.168.0.100";

        public Settings()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            if (settings.Contains(Key))
            {
                if (settings[Key] != value)
                {
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }

        public void Save()
        {
            settings.Save();
        }


        public bool IsUseGyro
        {
            get
            {
                return GetValueOrDefault<bool>(IsUseGyroKeyName, IsUseGyroDefault);
            }
            set
            {
                if (AddOrUpdateValue(IsUseGyroKeyName, value))
                {
                    Save();
                }
            }
        }

        public Model.TurnMethod TurnMethod
        {
            get
            {
                return (Model.TurnMethod)GetValueOrDefault<int>(TurnMethodKeyName, TurnMethodDefault);
            }
            set
            {
                if (AddOrUpdateValue(TurnMethodKeyName, value))
                {
                    Save();
                }
            }
        }

        public string Port
        {
            get
            {
                return GetValueOrDefault<string>(PortKeyName, PortDefault);
            }
            set
            {
                if (AddOrUpdateValue(PortKeyName, value))
                {
                    Save();
                }
            }
        }

        public string IP
        {
            get
            {
                return GetValueOrDefault<string>(IPKeyName, IPDefault);
            }
            set
            {
                if (AddOrUpdateValue(IPKeyName, value))
                {
                    Save();
                }
            }
        }
    }
}
