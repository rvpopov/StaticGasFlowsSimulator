using System;
using Valkyrie.ComputationalModels.Models;
using Valkyrie.Sigryun.Calculators.StaticGGA;

namespace Valkyrie.ComputationalModels.CalculatorRealisations.StaticGGACalculator
{
    public class StaticGGABridge : BridgeModel, IStaticGGAComputationalModel
    {
        public double CalculateInitialFlow(double pin, double pout)
        {
            throw new Exception("Для объекта типа перемычки гидравлическая модель не реализуется");
        }

        public void CalculateFlowBorders(double pin, double pout, out double qmin, out double qmax)
        {
            throw new Exception("Для объекта типа перемычки гидравлическая модель не реализуется");
        }

        public double CalculatePressureLossDifferential(double pin, double pout, double qin)
        {
            throw new Exception("Для объекта типа перемычки гидравлическая модель не реализуется");
        }

        public double CalculatePressureLoss(double pin, double pout, double qin)
        {
            throw new Exception("Для объекта типа перемычки гидравлическая модель не реализуется");
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
            throw new Exception("Для объекта типа перемычки гидравлическая модель не реализуется");
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
            throw new Exception("Для объекта типа перемычки гидравлическая модель не реализуется");
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
        /// Производит обновление параметров модели по результатам расчета
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="q">Расход на входе дуги</param>
        public void FinalizeHydraulic(double pin, double pout, double q)
        {
            throw new Exception("Для объекта типа перемычки гидравлическая модель не реализуется");
        }

        public object Tag
        {
            get;
            set;
        }

        public bool IsDisabled
        {
            get
            {
                return true;
            }
        }

        public bool IsIgnorable
        {
            get
            {
                return true;
            }
        }

        public Sigryun.IComputationalModel Clone()
        {
            throw new NotImplementedException();
        }
    }
}
