using Microsoft.Devices.Sensors;

namespace robobot_winphone.Model.SensorHandler
{
    public abstract class AbstractCompassSensorHandler : AbstractSensorHandler
    {
        protected Compass Compass;

        protected void CompassCalibrate()
        {
            Stop();
            NavigationManager.Instance.NavigateToCalibrationPage();
        }
    }
}
