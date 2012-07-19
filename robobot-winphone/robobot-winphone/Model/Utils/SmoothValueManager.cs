using System;

namespace robobot_winphone.Model.Utils
{
    public class SmoothValueManager
    {
        private const double ArrivedEps = 0.65;
        private const double LeavedEps = 2.5;
        private const double SpeedEps = 0.55;

        private double goalDirection;
        private double needleDirection;
        private double speed;

        private bool isArrived; // The needle has not arrived the goalDirection

        public double GetSmoothValue(double newValue)
        {
            CalculateGoalDirection(newValue);
            if (IsNeedPainting())
            {
                isArrived = false;
                var difference = CalculateNormalDifference(needleDirection, goalDirection);
                speed = CalculateSpeed(difference, speed);
                needleDirection = needleDirection + speed;

                return needleDirection;
            }
            else
            {
                isArrived = true;
                return needleDirection;
            }

        }

        private void CalculateGoalDirection(double newValue)
        {
            var difference = newValue - goalDirection;
            difference = NormalizeValue(difference);

            newValue = goalDirection + difference / 4;
            newValue = NormalizeValue(newValue);

            goalDirection = newValue;
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
                return Math.Abs(needleDirection - goalDirection) > LeavedEps;
            }
            else
            {
                return Math.Abs(needleDirection - goalDirection) > ArrivedEps || Math.Abs(speed) > SpeedEps;
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
