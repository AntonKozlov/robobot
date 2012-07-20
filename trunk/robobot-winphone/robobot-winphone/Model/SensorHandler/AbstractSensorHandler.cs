using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model.Utils;

namespace robobot_winphone.Model.SensorHandler
{
    public abstract class AbstractSensorHandler
    {
        protected Accelerometer Accelerometer;
        protected DispatcherTimer Timer;
        protected ISensorView SensorView;

        protected SmoothValueManager TurnSmoothValueManager;
        protected SmoothValueManager SpeedSmoothValueManager;

        private const int MaxValue = 100;

        public abstract void Start();
        public abstract void Stop();

        protected virtual int CalculateSpeed(double value, double factor)
        {
            return CalculateValue(value, SpeedSmoothValueManager, factor);
        }

        protected virtual int CalculateTurn(double value, double factor)
        {
            return CalculateValue(value, TurnSmoothValueManager, factor);
        }

        protected virtual int CalculateSpeed(double value)
        {
            return CalculateValue(value, SpeedSmoothValueManager, 1);
        }

        protected virtual int CalculateTurn(double value)
        {
            return CalculateValue(value, TurnSmoothValueManager, 1);
        }

        protected int CalculateValue(double value, SmoothValueManager smoothValueManager, double factor)
        {
            var outPutValue = value;

            outPutValue = smoothValueManager.GetSmoothValue(outPutValue) * factor;

            if (outPutValue >= MaxValue)
            {
                return MaxValue;
            }
            if (outPutValue <= -MaxValue)
            {
                return -MaxValue;
            }
            return (int)outPutValue;
        }
    }
}
