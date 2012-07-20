using System;
using System.IO.IsolatedStorage;


namespace robobot_winphone.Model
{
    public enum TurnMethod
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
        const int TurnMethodDefault = (int)TurnMethod.Inclination;
        const string PortDefault = "43214";
        const string IPDefault = "192.168.0.100";

        public Settings()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public bool AddOrUpdateValue(string key, Object value)
        {
            var valueChanged = false;

            if (settings.Contains(key))
            {
                if (settings[key] != value)
                {
                    settings[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                settings.Add(key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;

            if (settings.Contains(key))
            {
                value = (T)settings[key];
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
                return GetValueOrDefault(IsUseGyroKeyName, IsUseGyroDefault);
            }
            set
            {
                if (AddOrUpdateValue(IsUseGyroKeyName, value))
                {
                    Save();
                }
            }
        }

        public TurnMethod TurnMethod
        {
            get
            {
                return (TurnMethod)GetValueOrDefault(TurnMethodKeyName, TurnMethodDefault);
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
                return GetValueOrDefault(PortKeyName, PortDefault);
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
                return GetValueOrDefault(IPKeyName, IPDefault);
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
