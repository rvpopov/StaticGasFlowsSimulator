using System;
using Valkyrie.ComputationalModels.Models;
using Valkyrie.Sigryun;
using Valkyrie.Sigryun.Interfaces;

namespace Valkyrie.ComputationalModels.CalculatorRealisations.StaticGGACalculator
{
    public class StaticGGADustCleaner : LocalResistanceModel, IHydraulicNodeFinalize, IComputationalModel
    //IStaticGGAComputationalModel, ITermalComputationalModel
    {
        /// <summary>
        /// Производит расчета начального приближения по расходу
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <returns>Начальное приближение по расходу</returns>
        public double CalculateInitialFlow(double pin, double pout)
        {
            this.GetCoefficientA(pin, pout);
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

        ///// <summary>
        ///// Производит обновление параметров модели по результатам расчета
        ///// </summary>
        ///// <param name="pin">Давление в начале дуги</param>
        ///// <param name="pout">Давление в конце дуги</param>
        ///// <param name="q">Расход на входе дуги</param>
        //public void FinalizeHydraulic(double pin, double pout, double q)
        //{
        //    PressureStart = pin;
        //    PressureFinish = pout;
        //    FlowStart = FlowFinish = q;
        //}

        /// <summary>
        /// <summary>
        /// Производит обновление параметров модели по результатам расчета
        /// </summary>
        /// <param name="p">Давление в узле</param>
        /// <param name="t">Температура в узле</param>
        /// <param name="qin">Расход поступающий в узел</param>
        /// <param name="qout">Расход исходящий из узла</param>
        public void FinalizeHydraulic(double p, double t, double qin, double qout)
        {
            Pressure = p;
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

        ///// <summary>
        ///// Вычисляет температуру в конце объекта
        ///// </summary>
        ///// <param name="pin">Давление в начале</param>
        ///// <param name="pout">Давление в конце</param>
        ///// <param name="qin">Расход в начале</param>
        ///// <param name="qout">Расход в конце</param>
        ///// <param name="tin">Давление в начале</param>
        ///// <returns></returns>
        //public virtual double CalculateFinishTemperature(double pin, double pout, double qin, double qout, double tin)
        //{
        //    //TemperatureStart = tin;
        //    //TemperatureFinish = tin;
        //    //return TemperatureFinish;
        //}

        #region Свойства

        #region Результаты расчета

        //public double PressureStart
        //{
        //    get;
        //    protected set;
        //}

        //public double PressureFinish
        //{
        //    get;
        //    protected set;
        //}

        //public double FlowStart
        //{
        //    get;
        //    protected set;
        //}

        //public double FlowFinish
        //{
        //    get;
        //    protected set;
        //}

        //public double TemperatureStart
        //{
        //    get;
        //    protected set;
        //}

        //public double TemperatureFinish
        //{
        //    get;
        //    protected set;
        //}

        //public double PressureLoss
        //{
        //    get
        //    {
        //        return PressureStart - PressureFinish;
        //    }
        //}

        public double Pressure
        {
            get;
            private set;
        }

        #endregion

        #endregion

        public object Tag
        {
            get;
            set;
        }

        public bool IsDisabled
        {
            get
            {
                return false;
            }
        }

        public bool IsIgnorable
        {
            get
            {
                return true;
            }
        }

        public new Sigryun.IComputationalModel Clone()
        {
            throw new NotImplementedException();
        }
    }
}
