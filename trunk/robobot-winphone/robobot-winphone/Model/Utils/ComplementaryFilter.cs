using System;
using Microsoft.Xna.Framework;

namespace robobot_winphone.Model.Utils
{
    public class ComplementaryFilter
    {      
        private const float timeToDeviceNormalization = 0.1f;

        private float delta;
        private DateTimeOffset lastUpdateTime = DateTimeOffset.MinValue;

        public Vector3 CummulativeValue { get; private set; }

        public ComplementaryFilter(float timeBetweenUpdates)
        {
            delta = timeBetweenUpdates / (timeBetweenUpdates + timeToDeviceNormalization);
        }

        public void UpdateCummulativeValue(Vector3 gyroscopeData, Vector3 acclerometerData, Vector3 compassData, DateTimeOffset currentUpdateTime)
        {
            var timeSinceLastUpdate = (float)(currentUpdateTime - lastUpdateTime).TotalSeconds;

            var pitch = (float)Math.Atan2(-acclerometerData.Y, -acclerometerData.Z);
            var roll = (float)Math.Atan2(acclerometerData.X, Math.Sqrt(acclerometerData.Y * acclerometerData.Y + 
                acclerometerData.Z * acclerometerData.Z));
            var heading = (float)Math.Atan2(-(compassData.X * Math.Cos(roll) + compassData.Z * Math.Sin(roll)),
                compassData.X * Math.Sin(pitch) * Math.Sin(roll) + compassData.Y * Math.Cos(pitch) -
                compassData.Z * Math.Sin(pitch) * Math.Cos(roll));

            var correction = new Vector3(roll, pitch, heading);

            var integratedGyro = gyroscopeData * timeSinceLastUpdate;

            CummulativeValue = (1 - delta) * (CummulativeValue + integratedGyro) + delta * correction;

            lastUpdateTime = currentUpdateTime;
        }
    }
}
