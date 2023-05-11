using System;
using Valkyrie.Sigryun.Calculators.StaticGGA;
using Valkyrie.Sigryun.Calculators.TermalCalculator;

namespace Valkyrie.ComputationalModels.CalculatorRealisations.StaticGGACalculator
{
    public class StaticGGALossObject : IStaticGGAComputationalModel, ITermalComputationalModel
    {
        #region Свойства

        public object Tag
        {
            get;
            set;
        }

        public bool IsDisabled
        {
            get;
            set;
        }

        public bool IsIgnorable
        {
            get
            {
                return false;
            }
        }

        public double PressureLoss
        {
            get;
            set;
        }

        public double TemperatureSetpoint
        {
            get;
            set;
        }

        public double TemperatureLoss
        {
            get;
            set;
        }

        public TemperatureBoundaries TemperatureBoundary
        {
            get;
            set;
        }

        #endregion

        #region Методы

        public double CalculateInitialFlow(double pin, double pout)
        {
            return 50;
        }

        public void CalculateFlowBorders(double pin, double pout, out double qmin, out double qmax)
        {
            qmin = double.NegativeInfinity;
            qmax = double.PositiveInfinity;
        }

        public double CalculatePressureLoss(double pin, double pout, double qin)
        {
            return (pin + pout) * PressureLoss;
        }

        public double CalculateFlowLoss(double pin, double pout, double qin)
        {
            return 0;
        }

        public double CalculatePressureLossDifferential(double pin, double pout, double qin)
        {
            return 0.1;
        }

        public double CalculatePressureInDifferential(double pin, double pout, double qin)
        {
            return 0;
        }

        public double CalculatePressureOutDifferential(double pin, double pout, double qin)
        {
            return 0;
        }

        public Sigryun.IComputationalModel Clone()
        {
            throw new NotImplementedException();
        }

        public void FinalizeHydraulic(double pin, double pout, double qin)
        {

        }

        public double CalculateFinishTemperature(double pin, double pout, double qin, double qout, double tin)
        {
            switch (TemperatureBoundary)
            {
                case TemperatureBoundaries.Setpoint:
                    return TemperatureSetpoint;
                case TemperatureBoundaries.Loss:
                    return tin - TemperatureLoss;
                default:
                    return tin;
            }
        }

        #endregion

        #region Перечисления

        public enum TemperatureBoundaries
        {
            Setpoint,
            Loss
        }

        #endregion
    }
}
