using System;
using System.Windows.Threading;
using robobot_winphone.Model;

namespace robobot_winphone.Model.Utils
{
    public class SmoothValueManager
    {
        private const int LongSleep = 10;
        private const int DefaultSleep = 5;

        private const double ArrivedEps = 0.65;
        private const double LeavedEps = 2.5;
        private const double SpeedEps = 0.55;

        private DispatcherTimer timer;

        private double goalValue;
        private double value;
        private double speed;

        private bool isArrived; // The needle has not arrived the goalValue

        public SmoothValueManager()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(DefaultSleep)
            };

            timer.Tick += TimerTick;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (IsNeedPainting())
            {
                isArrived = false;
                var difference = CalculateNormalDifference(value, goalValue);
                speed = CalculateSpeed(difference, speed);
                value = value + speed;
                value = NormalizeValue(value);
            }
            else
            {
                isArrived = true;
            }

            timer.Interval = TimeSpan.FromMilliseconds(isArrived ? LongSleep : DefaultSleep);
        }

        public double GetSmoothValue(double newValue)
        {
            CalculateGoalValue(newValue);

            return value;
        }

        private void CalculateGoalValue(double newValue)
        {
            newValue = NormalizeValue(newValue);

            var difference = newValue - goalValue;
            difference = NormalizeValue(difference);

            newValue = goalValue + difference / 4;
            newValue = NormalizeValue(newValue);

            goalValue = newValue;
        }

        private static double CalculateSpeed(double difference, double oldSpeed)
        {
            double newSpeed;
            newSpeed = oldSpeed * 0.75;
            newSpeed += difference / 25.0;

            return newSpeed;
        }

        private bool IsNeedPainting()
        {
            if (isArrived)
            {
                return Math.Abs(value - goalValue) > LeavedEps;
            }
            else
            {
                return Math.Abs(value - goalValue) > ArrivedEps || Math.Abs(speed) > SpeedEps;
            }
        }

        public static double CalculateNormalDifference(double lastDirection, double currentDirection)
        {
            var difference = currentDirection - lastDirection;
            return NormalizeValue(difference);
        }

        public static double NormalizeValue(double value)
        {
            value %= 360;

            if (value < -180)
            {
                value += 360;
            }

            if (value > 180)
            {
                value -= 360;
            }
            return value;
        }
    }
}
