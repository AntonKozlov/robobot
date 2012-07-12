using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace robobot_winphone.Model.SensorHandler
{
    public abstract class AbstractSensorHandler
    {
        public abstract void Start();
        public abstract void Stop();

        //ToDo: method implementation
        protected bool IsCompassNeedCalipration()
        {
            return false;
        }
    }
}
