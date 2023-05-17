namespace Valkyrie.Sigryun.Calculators.TermalCalculator
{
    /// <summary>
    /// Представляет интерфейс модели объекта, с возможностью температурного расчета
    /// </summary>
    public interface ITermalComputationalModel
    {
        /// <summary>
        /// Вычисляет температуру в конце объекта
        /// </summary>
        /// <param name="pin">Давление в начале</param>
        /// <param name="pout">Давление в конце</param>
        /// <param name="qin">Расход в начале</param>
        /// <param name="qout">Расход в конце</param>
        /// <param name="tin">Давление в начале</param>
        /// <returns></returns>
        double CalculateFinishTemperature(double pin, double pout, double qin, double qout, double tin);
    }
}
