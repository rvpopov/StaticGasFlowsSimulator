using System;
using Valkyrie.ComputationalModels.Models;
using Valkyrie.Sigryun;
using Valkyrie.Sigryun.Calculators.StaticGGA;
using Valkyrie.Sigryun.Calculators.TermalCalculator;
using Valkyrie.Sigryun.Interfaces;

namespace Valkyrie.ComputationalModels.CalculatorRealisations.StaticGGACalculator
{
    public class StaticGGAGasCompressorUnit : GasCompressorUnitModel,
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

        public double CompressionRate
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
            double qmin, qmax;
            GetQMinQmaxAll(pin, SConverter.CToK(11), out qmin, out qmax);
            TemperatureStart = SConverter.CToK(10);
            return (qmin + qmax) / 2;
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
            GetQMinQmaxAll(pin, TemperatureStart, out qmin, out qmax);
            //qmin *= 0.9;
            //qmax *= 1.1;
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
            double pout1, tout, qfuel;

            Calculate(pin, qin, TemperatureStart, out pout1, out tout, out qfuel);
            var compressionRatio = pout1 / pin;

            //double res = (pin + pout) * pin * (1 - compressionRatio);
            double res = pin * pin - pout1 * pout1;

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
            double dq = 0.01;
            var f0 = CalculatePressureLoss(pin, pout, qin - dq);
            var f1 = CalculatePressureLoss(pin, pout, qin + dq);

            var res = (f1 - f0) / (2 * dq);
            if (Math.Abs(res) < 0.01)
                res = 0.01;
            return res;
        }

        /// <summary>
        /// Производит обновление параметров модели по результатам расчета
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="q">Расход на входе дуги</param>
        public void FinalizeHydraulic(double pin, double pout, double q)
        {
            double pout1, tout, qfuel;

            Calculate(pin, q, TemperatureStart, out pout1, out tout, out qfuel);

            PressureStart = pin;
            PressureFinish = pout;
            FlowStart = q;
            FlowFinish = q - qfuel;
            FuelFlow = qfuel;
            CompressionRate = pout / pin;
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
            double pout1, tout, qfuel;

            Calculate(pin, qin, TemperatureStart, out pout1, out tout, out qfuel);
            //qfuel = 0;
            return -qfuel;
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
        public virtual double CalculateFinishTemperature(double pin, double pout, double qin, double qout, double tin)
        {
            TemperatureStart = tin;
            double pout1, tout, qfuel;

            Calculate(pin, qin, TemperatureStart, out pout1, out tout, out qfuel);
            TemperatureFinish = tout;
            return TemperatureFinish;
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
            double du = 0.0001;
            double uin0 = pin * pin - du;
            double uin1 = pin * pin + du;

            var f0 = CalculatePressureLoss(Math.Sqrt(uin0), pout, qin);
            var f1 = CalculatePressureLoss(Math.Sqrt(uin1), pout, qin);

            var res = (f1 - f0) / (2 * du);
            return res;
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
            double du = 0.0001;
            double uout0 = pout * pout - du;
            double uout1 = pout * pout + du;

            var f0 = CalculatePressureLoss(pin, Math.Sqrt(uout0), qin);
            var f1 = CalculatePressureLoss(pin, Math.Sqrt(uout1), qin);

            var res = (f1 - f0) / (2 * du);
            return res;
        }

        /// <summary>
        /// Вызывает копирование объекта
        /// </summary>
        /// <returns></returns>
        public new IComputationalModel Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
