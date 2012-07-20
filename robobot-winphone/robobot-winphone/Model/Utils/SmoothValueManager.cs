using System;

namespace robobot_winphone.Model.Utils
{
    public class SmoothValueManager
    {
        private const double ArrivedEps = 0.65;
        private const double LeavedEps = 2.5;
        private const double SpeedEps = 0.55;

        private double goalValue;
        private double oldValue;
        private double speed;

        private bool isArrived; // The needle has not arrived the goalValue

        public double GetSmoothValue(double newValue)
        {
            CalculateGoalValue(newValue);

            if (IsNeedPainting())
            {
                isArrived = false;
                var difference = CalculateNormalDifference(oldValue, goalValue);
                speed = CalculateSpeed(difference, speed);
                oldValue = oldValue + speed;

                return oldValue;
            }
            else
            {
                isArrived = true;
                return oldValue;
            }

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
                return Math.Abs(oldValue - goalValue) > LeavedEps;
            }
            else
            {
                return Math.Abs(oldValue - goalValue) > ArrivedEps || Math.Abs(speed) > SpeedEps;
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
