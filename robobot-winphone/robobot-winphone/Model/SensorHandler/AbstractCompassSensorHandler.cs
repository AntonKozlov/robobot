using Microsoft.Devices.Sensors;

namespace robobot_winphone.Model.SensorHandler
{
    public abstract class AbstractCompassSensorHandler : AbstractSensorHandler
    {
        protected Compass compass;

        protected void CompassCalibrate(object sender, CalibrationEventArgs e)
        {
            Stop();
            NavigationManager.Instance.NavigateToCalibrationPage();
        }
    }
}
