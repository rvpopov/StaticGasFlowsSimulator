using System;
using Valkyrie.ComputationalModels.Models;
using Valkyrie.Sigryun;
using Valkyrie.Sigryun.Interfaces;

namespace Valkyrie.ComputationalModels.CalculatorRealisations.StaticGGACalculator
{
    public class StaticGGAValve : ValveModel, IComputationalModel, IHydraulicNodeFinalize
    {
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

        public object Tag
        {
            get;
            set;
        }

        public bool IsDisabled
        {
            get
            {
                return State == Enums.ValveStates.Close;
            }
        }

        public bool IsIgnorable
        {
            get
            {
                return State == Enums.ValveStates.Open;
            }
        }

        public double Pressure
        {
            get;
            private set;
        }

        public Sigryun.IComputationalModel Clone()
        {
            throw new NotImplementedException();
        }

    }
}
