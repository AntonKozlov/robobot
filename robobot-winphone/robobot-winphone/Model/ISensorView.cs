using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace robobot_winphone.Model
{
    public interface ISensorView
    {
        void ProcessSensorData(int turn, int speed);
    }
}
