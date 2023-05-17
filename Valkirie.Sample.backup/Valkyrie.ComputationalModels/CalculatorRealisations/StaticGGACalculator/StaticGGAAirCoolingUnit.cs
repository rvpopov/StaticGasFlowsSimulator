using Valkyrie.Sigryun.Interfaces;

namespace Valkyrie.ComputationalModels.CalculatorRealisations.StaticGGACalculator
{
    public class StaticGGAAirCoolingUnit : StaticGGADustCleaner, ITemperatureSetpoint
    {
        #region Свойства

        public double TemperatureSetpoint
        {
            get;
            set;
        }

        //public double TemperatureLoss
        //{
        //    get
        //    {
        //        return TemperatureStart - TemperatureFinish;
        //    }
        //}

        #endregion

        #region Методы

        ///// <summary>
        ///// Вычисляет температуру в конце объекта
        ///// </summary>
        ///// <param name="pin">Давление в начале</param>
        ///// <param name="pout">Давление в конце</param>
        ///// <param name="qin">Расход в начале</param>
        ///// <param name="qout">Расход в конце</param>
        ///// <param name="tin">Давление в начале</param>
        ///// <returns></returns>
        //public override double CalculateFinishTemperature(double pin, double pout, double qin, double qout, double tin)
        //{
        //    TemperatureStart = tin;
        //    TemperatureFinish = TemperatureSetpoint;
        //    return TemperatureFinish;
        //}

        #endregion
    }
}
