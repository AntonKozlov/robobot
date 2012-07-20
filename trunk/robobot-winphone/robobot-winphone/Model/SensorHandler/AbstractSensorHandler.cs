using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model.Utils;

namespace robobot_winphone.Model.SensorHandler
{
    public abstract class AbstractSensorHandler
    {
        protected Accelerometer accelerometer;
        protected DispatcherTimer timer;
        protected ISensorView sensorView;

        protected SmoothValueManager turnSmoothValueManager;
        protected SmoothValueManager speedSmoothValueManager;

        private const int MaxValue = 100;

        public abstract void Start();
        public abstract void Stop();

        protected virtual int CalculateSpeed(double value)
        {
            return CalculateValue(value, speedSmoothValueManager);
        }

        protected virtual int CalculateTurn(double value)
        {
            return CalculateValue(value, turnSmoothValueManager);
        }

        protected int CalculateValue(double value, SmoothValueManager smoothValueManager)
        {
            var outPutValue = value;

            outPutValue = smoothValueManager.GetSmoothValue(outPutValue);

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
