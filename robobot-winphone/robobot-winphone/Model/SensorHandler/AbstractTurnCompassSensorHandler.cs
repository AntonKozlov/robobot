namespace robobot_winphone.Model.SensorHandler
{
    public abstract class AbstractTurnCompassSensorHandler : AbstractCompassSensorHandler
    {
        protected double fixCompassData;
        protected bool isFixComassDataDetected;

        protected override int CalculateTurn(double value, double factor)
        {
            return CalculateValue(value - fixCompassData, TurnSmoothValueManager, factor);
        }

        protected override int CalculateTurn(double value)
        {
            return CalculateValue(value - fixCompassData, TurnSmoothValueManager, 1);
        }
    }
}
