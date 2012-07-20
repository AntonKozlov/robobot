using System;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model.Utils;

namespace robobot_winphone.Model.SensorHandler
{
    public class ACSensorHandler : AbstractSensorHandler
    {
        private Accelerometer accelerometer;
        private Compass compass;
        private ISensorView sensorView;
        private DispatcherTimer timer;

        private SmoothValueManager turnSmoothValueManager;
        private SmoothValueManager speedSmoothValueManager;

        private DateTime startTime;
        private double fixCompassData;
        private bool isFixComassDataDetected;

        public ACSensorHandler(double frequency, ISensorView sensorView)
        {
            if (Accelerometer.IsSupported && Compass.IsSupported)
            {
                accelerometer = new Accelerometer();
                compass = new Compass();

                turnSmoothValueManager = new SmoothValueManager();
                speedSmoothValueManager = new SmoothValueManager();

                this.sensorView = sensorView;

                compass.Calibrate += new EventHandler<CalibrationEventArgs>(CompassCalibrate);

                accelerometer.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);
                compass.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);

                timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(frequency) };
                timer.Tick += TimerTick;
            }
            else
            {
                LogManager.Log("Some sensor is not supported on this device");
            }            
        }

        public override void Start()
        {
            compass.Start();
            accelerometer.Start();
            timer.Start();
            startTime = DateTime.Now;
            isFixComassDataDetected = false;
        }

        public override void Stop()
        {
            compass.Stop();
            accelerometer.Stop();
            timer.Stop();
        }

        private void CompassCalibrate(object sender, CalibrationEventArgs e)
        {
            Stop();
            NavigationManager.Instance.NavigateToCalibrationPage();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (isFixComassDataDetected)
            {
                sensorView.ProcessSensorData(CalculateTurn((int)compass.CurrentValue.TrueHeading),
                                CalculateValue((double)(-accelerometer.CurrentValue.Acceleration.X)));
            }
            else if ((DateTime.Now - startTime).Seconds > 1)
            {
                fixCompassData = compass.CurrentValue.TrueHeading;
                isFixComassDataDetected = true;
            }
        }
        private const int MaxValue = 100;

        private int CalculateValue(double value)
        {
            var outPutValue = value * MaxValue * 1.8;
            outPutValue = speedSmoothValueManager.GetSmoothValue(outPutValue);
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

        private int CalculateTurn(double value)
        {
            var outPutValue = value - fixCompassData;

            outPutValue = turnSmoothValueManager.GetSmoothValue(outPutValue);

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
