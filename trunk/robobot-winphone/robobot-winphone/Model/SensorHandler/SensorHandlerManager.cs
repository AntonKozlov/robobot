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

namespace robobot_winphone.Model.SensorHandler
{
    public class SensorHandlerManager
    {
        public static AbstractSensorHandler GetSensorHandler(double frequency, ISensorView sensorView)
        {
            Settings settings = new Settings();
            switch (settings.IsUseGyro)
            {
                case true:
                    {
                        switch (settings.TurnMethod)
                        {
                            case (TurnMethod.Inclination): return new AGSensorHandler(frequency, sensorView);
                            case (TurnMethod.Rotation): return new ACGSensorHandler(frequency, sensorView);
                        }
                        break;
                    };
                case false:
                    {
                        switch (settings.TurnMethod)
                        {
                            case (TurnMethod.Inclination): return new ASensorHandler(frequency, sensorView);
                            case (TurnMethod.Rotation): return new ACSensorHandler(frequency, sensorView);
                        }
                        break;
                    };
            }
            return null;
        }
    }
}
