using System;
using Valkyrie.ComputationalModels.Models;
using Valkyrie.Sigryun.Calculators.StaticGGA;
using Valkyrie.Sigryun.Calculators.TermalCalculator;

namespace Valkyrie.ComputationalModels.CalculatorRealisations.StaticGGACalculator
{
    public class StaticGGAPipeline : PipelineModel,
        IStaticGGAComputationalModel, ITermalComputationalModel
    {
        #region Свойства

        #region Базовые свойства

        /// <summary>
        /// Получает или задает тег объекта
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Признак того, что дуга должна быть удалена из графа
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Получает признак того, что дуга должна быть эквивалентирована в узел
        /// </summary>
        public bool IsIgnorable
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Результаты расчета

        /// <summary>
        /// Получает расчетное давление в начале объекта
        /// </summary>
        public double PressureStart
        {
            get;
            private set;
        }

        /// <summary>
        /// Получает расчетное давление в конце объекта
        /// </summary>
        public double PressureFinish
        {
            get;
            private set;
        }

        /// <summary>
        /// Получает расчетный расход в начале объекта
        /// </summary>
        public double FlowStart
        {
            get;
            private set;
        }

        /// <summary>
        /// Полуачет расчетный расхода в конце объекта
        /// </summary>
        public double FlowFinish
        {
            get;
            private set;
        }

        /// <summary>
        /// Получает расчетную температуру в начале объекта
        /// </summary>
        public double TemperatureStart
        {
            get;
            private set;
        }

        /// <summary>
        /// Получает расчетную температуру в конце объекта
        /// </summary>
        public double TemperatureFinish
        {
            get;
            private set;
        }

        #endregion

        #endregion

        #region Методы

        /// <summary>
        /// Производит расчета начального приближения по расходу
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <returns>Начальное приближение по расходу</returns>
        public double CalculateInitialFlow(double pin, double pout)
        {
            pout = pin * 0.95;
            TAvg = 280;
            A = GetCoefficientA(pin, pout, 1, TAvg);
            return Math.Sqrt(Math.Abs(pin * pin - pout * pout) / A) * Math.Sign(pin - pout);
        }

        /// <summary>
        /// Производит расчет минимального и максимального значения расхода через дугу
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qmin">Минимальное значение расхода через объект</param>
        /// <param name="qmax">Максимальное значение расхода через объект</param>
        public void CalculateFlowBorders(double pin, double pout, out double qmin, out double qmax)
        {
            qmin = double.MinValue;
            qmax = double.MaxValue;
        }

        /// <summary>
        /// Производит расчет производной замыкающего соотношения по расходу на входе дуги
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qin">Расход на входе дуги</param>
        /// <returns>Значение производный замыкающего соотношения по расходу в начале дуги</returns>
        public double CalculatePressureLoss(double pin, double pout, double qin)
        {
            double res = A * qin * Math.Abs(qin);
            return res;
        }

        /// <summary>
        /// Производит расчет значения функции замыкающего соотношения
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qin">Расход на входе дуги</param>  
        /// <returns>Значение функции замыкающего соотношения</returns>
        public double CalculatePressureLossDifferential(double pin, double pout, double qin)
        {
            double res = 2 * A * Math.Abs(qin);
            if (res < 0.01)
                return 2 * 0.1;
            return res;
        }

        /// <summary>
        /// Производит расчет производной замыкающего соотношения по давлению в начале дуги
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qin">Расход на входе дуги</param>
        /// <returns>Значение производный замыкающего соотношения по давлению в начале дуги</returns>
        public double CalculatePressureInDifferential(double pin, double pout, double qin)
        {
            return 0;
        }

        /// <summary>
        /// Производит расчет производной замыкающего соотношения по давлению в конце дуги
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qin">Расход на входе дуги</param>
        /// <returns>Значение производный замыкающего соотношения по давлению в конце дуги</returns>
        public double CalculatePressureOutDifferential(double pin, double pout, double qin)
        {
            return 0;
        }

        /// <summary>
        /// Производит обновление параметров модели по результатам расчета
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="q">Расход на входе дуги</param>
        public void FinalizeHydraulic(double pin, double pout, double q)
        {
            PressureStart = pin;
            PressureFinish = pout;
            FlowStart = FlowFinish = q;
            A = GetCoefficientA(pin, pout, q, TAvg);
        }

        /// <summary>
        /// Производит расчет значения потери расхода на дуге
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qin">Расход на входе дуги</param>  
        /// <returns>Значение функции замыкающего соотношения</returns>
        public double CalculateFlowLoss(double pin, double pout, double qin)
        {
            return 0;
        }

        /// <summary>
        /// Вызывает копирование объекта
        /// </summary>
        /// <returns></returns>
        public new Sigryun.IComputationalModel Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Вычисляет температуру в конце объекта
        /// </summary>
        /// <param name="pin">Давление в начале</param>
        /// <param name="pout">Давление в конце</param>
        /// <param name="qin">Расход в начале</param>
        /// <param name="qout">Расход в конце</param>
        /// <param name="tin">Давление в начале</param>
        /// <returns></returns>
        public double CalculateFinishTemperature(double pin, double pout, double qin, double qout, double tin)
        {
            TemperatureStart = tin;
            TemperatureFinish = GetTout(pin, pout, tin, qin);
            A = GetCoefficientA(pin, pout, qin, TAvg);
            return TemperatureFinish;
        }

        #endregion
    }
}
