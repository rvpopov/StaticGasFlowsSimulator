using Valkyrie.Sigryun.Interfaces;

namespace Valkyrie.Sigryun.Calculators.StaticGGA
{
    public interface IStaticGGAComputationalModel : IComputationalModel, IHydraulicArcFinalize
    {
        /// <summary>
        /// Производит расчета начального приближения по расходу
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <returns>Начальное приближение по расходу</returns>
        double CalculateInitialFlow(double pin, double pout);

        /// <summary>
        /// Производит расчет минимального и максимального значения расхода через дугу
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qmin">Минимальное значение расхода через объект</param>
        /// <param name="qmax">Максимальное значение расхода через объект</param>
        void CalculateFlowBorders(double pin, double pout, out double qmin, out double qmax);

        /// <summary>
        /// Производит расчет значения функции замыкающего соотношения
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qin">Расход на входе дуги</param>  
        /// <returns>Значение функции замыкающего соотношения</returns>
        double CalculatePressureLoss(double pin, double pout, double qin);

        /// <summary>
        /// Производит расчет значения потери расхода на дуге
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qin">Расход на входе дуги</param>  
        /// <returns>Значение функции замыкающего соотношения</returns>
        double CalculateFlowLoss(double pin, double pout, double qin);

        #region Расчет производных

        /// <summary>
        /// Производит расчет производной замыкающего соотношения по расходу на входе дуги
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qin">Расход на входе дуги</param>
        /// <returns>Значение производный замыкающего соотношения по расходу в начале дуги</returns>
        double CalculatePressureLossDifferential(double pin, double pout, double qin);

        /// <summary>
        /// Производит расчет производной замыкающего соотношения по давлению в начале дуги
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qin">Расход на входе дуги</param>
        /// <returns>Значение производный замыкающего соотношения по давлению в начале дуги</returns>
        double CalculatePressureInDifferential(double pin, double pout, double qin);

        /// <summary>
        /// Производит расчет производной замыкающего соотношения по давлению в конце дуги
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qin">Расход на входе дуги</param>
        /// <returns>Значение производный замыкающего соотношения по давлению в конце дуги</returns>
        double CalculatePressureOutDifferential(double pin, double pout, double qin);

        #endregion
    }
}
