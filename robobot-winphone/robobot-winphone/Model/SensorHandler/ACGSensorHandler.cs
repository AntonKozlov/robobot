using System;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model.Utils;

namespace robobot_winphone.Model.SensorHandler
{
    public class ACGSensorHandler : AbstractSensorHandler
    {
        private Gyroscope gyroscope;
        private Accelerometer accelerometer;
        private Compass compass;
        private ComplementaryFilter filter;
        private ISensorView sensorView;
        private DispatcherTimer timer;

        private DateTime startTime;
        private SmoothValueManager turnSmoothValueManager;
        private SmoothValueManager speedSmoothValueManager;
        private double fixCompassData;
        private bool isFixComassDataDetected;

        public ACGSensorHandler(double frequency, ISensorView sensorView)
        {
            if (Gyroscope.IsSupported && Accelerometer.IsSupported && Compass.IsSupported)
            {
                filter = new ComplementaryFilter((float)frequency);
                gyroscope = new Gyroscope();
                accelerometer = new Accelerometer();
                compass = new Compass();
                turnSmoothValueManager = new SmoothValueManager();
                speedSmoothValueManager = new SmoothValueManager();
                this.sensorView = sensorView;

                accelerometer.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);
                compass.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);
                gyroscope.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);

                compass.Calibrate += new EventHandler<CalibrationEventArgs>(CompassCalibrate);

                accelerometer.CurrentValueChanged += accelerometer_CurrentValueChanged;

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
            gyroscope.Start();
            accelerometer.Start();
            timer.Start();
            startTime = DateTime.Now;
            isFixComassDataDetected = false;
        }

        public override void Stop()
        {
            compass.Stop();
            gyroscope.Stop();
            accelerometer.Stop();
            timer.Stop();
        }

        private void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            try
            {
                filter.UpdateCummulativeValue(gyroscope.CurrentValue.RotationRate, e.SensorReading.Acceleration,
                    compass.CurrentValue.MagnetometerReading, e.SensorReading.Timestamp);
            }
            catch (Exception)
            {
                LogManager.Log("Sensors reading error");
            }
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
                sensorView.ProcessSensorData(CalculateTurn(compass.CurrentValue.TrueHeading),
                    CalculateSpeed(-filter.CummulativeValue.X));
            }
            else if ((DateTime.Now - startTime).Seconds > 1)
            {
                //fixCompassData = filter.CummulativeValue.Z;
                fixCompassData = compass.CurrentValue.TrueHeading;
                isFixComassDataDetected = true;
            }
        }

        private const int MaxValue = 100;

        private int CalculateSpeed(double value)
        {
            var outPutValue = value * 60;

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
