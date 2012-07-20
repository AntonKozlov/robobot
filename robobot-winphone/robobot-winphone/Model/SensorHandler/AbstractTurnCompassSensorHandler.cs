using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Devices.Sensors;

namespace robobot_winphone.Model.SensorHandler
{
    public abstract class AbstractTurnCompassSensorHandler : AbstractCompassSensorHandler
    {
        protected double fixCompassData;
        protected bool isFixComassDataDetected;

        protected override int CalculateTurn(double value)
        {
            return CalculateValue(value - fixCompassData, turnSmoothValueManager);
        }
    }
}
