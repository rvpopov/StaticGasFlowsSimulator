using System;
using Valkyrie.Sigryun;

namespace Valkyrie.ComputationalModels.Models
{
    /// <summary>
    /// Представлет класс местного оспротивления
    /// </summary>
    public class LocalResistanceModel
    {
        #region Конструктор

        /// <summary>
        /// Создает объект местного сопротивления
        /// </summary>
        public LocalResistanceModel()
        {
            FlowProps = new FlowProperties(0.687);
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получает или задает коэффициент местного сопротивления
        /// </summary>
        public double Resistance
        {
            get;
            set;
        }

        //TODO: надо что-то делать с этой батой
        public FlowProperties FlowProps
        {
            get;
            private set;
        }

        public double A
        {
            get;
            private set;
        }

        #endregion

        #region Методы

        public double CaluclatePin(double pout, double q)
        {
            GetCoefficientA(pout, pout);
            return Math.Sqrt(pout * pout + A * q * q);
        }

        public double CaluclatePout(double pin, double q)
        {
            GetCoefficientA(pin, pin);
            return Math.Sqrt(pin * pin - A * q * q);
        }

        protected void GetCoefficientA(double pin, double pout)
        {
            A = 0.0016 * Resistance;
        }

        #endregion

    }
}
