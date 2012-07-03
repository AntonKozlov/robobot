using System;
using System.Windows;
using System.Windows.Media;

using Microsoft.Xna.Framework;

namespace robobot_winphone.Model
{
    public class ComplementaryFilter
    {
        
        private const float timeToDeviceNormalization = (float)0.1;

        private float delta;
        private DateTimeOffset lastUpdateTime = DateTimeOffset.MinValue;
        private Vector3 cummulativeValue;

        public Vector3 CummulativeValue
        {
            get
            {
                return this.cummulativeValue;
            }
        }

        public ComplementaryFilter(float timeBetweenUpdates)
        {
            delta = timeBetweenUpdates / (timeBetweenUpdates + timeToDeviceNormalization);
        }

        public void UpdateCummulativeValue(Vector3 gyroscopeData, Vector3 acclerometerData, Vector3 compassData, DateTimeOffset currentUpdateTime)
        {
            float timeSinceLastUpdate = (float)(currentUpdateTime - lastUpdateTime).TotalSeconds;

            float pitch = (float)Math.Atan2(-acclerometerData.Y, -acclerometerData.Z);
            float roll = (float)Math.Atan2(acclerometerData.X, Math.Sqrt(acclerometerData.Y * acclerometerData.Y + 
                acclerometerData.Z * acclerometerData.Z));
            float heading = (float)Math.Atan2(-(compassData.X * Math.Cos(roll) + compassData.Z * Math.Sin(roll)),
                compassData.X * Math.Sin(pitch) * Math.Sin(roll) + compassData.Y * Math.Cos(pitch) -
                compassData.Z * Math.Sin(pitch) * Math.Cos(roll));

            Vector3 correction = new Vector3(roll, pitch, heading);

            Vector3 integratedGyro = gyroscopeData * timeSinceLastUpdate;

            Vector3 gyro = new Vector3((float)(Math.Sin(pitch) / Math.Cos(pitch) * gyroscopeData.Y + gyroscopeData.Z),
                (float)(Math.Cos(roll) * gyroscopeData.Y - Math.Sin(pitch) * Math.Cos(roll) / Math.Cos(pitch) * gyroscopeData.Z),
                (float)(Math.Cos(roll) / Math.Cos(pitch) * gyroscopeData.X +
                Math.Sin(roll) * Math.Sin(pitch) / Math.Cos(pitch) * gyroscopeData.Y +
                Math.Cos(roll) * gyroscopeData.Z));

            cummulativeValue.X = (1 - delta) * (cummulativeValue.X + integratedGyro.X) + delta * correction.X;
            cummulativeValue.Y = (1 - delta) * (cummulativeValue.Y + integratedGyro.Y) + delta * correction.Y;
            cummulativeValue.Z = (1 - delta) * (cummulativeValue.Z + integratedGyro.Z) + delta * correction.Z;

            lastUpdateTime = currentUpdateTime;
        }
    }
}
