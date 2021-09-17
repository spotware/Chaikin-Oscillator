using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ChaikinOscillator : Indicator
    {
        private IndicatorDataSeries _accumulationDistribution;

        private MovingAverage _fastMa, _slowMa;

        [Parameter("Type", DefaultValue = MovingAverageType.Exponential, Group = "Fast MA")]
        public MovingAverageType FastMaType { get; set; }

        [Parameter("Periods", DefaultValue = 3, Group = "Fast MA")]
        public int FastMaPeriods { get; set; }

        [Parameter("Type", DefaultValue = MovingAverageType.Exponential, Group = "Slow MA")]
        public MovingAverageType SlowMaType { get; set; }

        [Parameter("Periods", DefaultValue = 10, Group = "Slow MA")]
        public int SlowMaPeriods { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _accumulationDistribution = CreateDataSeries();

            _fastMa = Indicators.MovingAverage(_accumulationDistribution, FastMaPeriods, FastMaType);
            _slowMa = Indicators.MovingAverage(_accumulationDistribution, SlowMaPeriods, SlowMaType);
        }

        public override void Calculate(int index)
        {
            var bar = Bars[index];

            var barRange = bar.High - bar.Low;

            var moneyFlowMultiplier = barRange == 0 ? 0 : (bar.Close - bar.Low - (bar.High - bar.Close)) / barRange;

            var moneyFlowVolume = moneyFlowMultiplier * bar.TickVolume;

            _accumulationDistribution[index] = index == 0
                ? moneyFlowVolume
                : _accumulationDistribution[index - 1] + moneyFlowVolume;

            Result[index] = _fastMa.Result[index] - _slowMa.Result[index];
        }
    }
}