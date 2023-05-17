namespace Valkyrie.HierarchicalModel.Models
{
    /// <summary>
    /// Класс представляет виртуальный поставщик газа
    /// </summary>
    public class VirtualEnterModel : FlowSourceBase
    {
        /// <summary>
        /// <summary>
        /// Производит обновление параметров модели по результатам расчета
        /// </summary>
        /// <param name="p">Давление в узле</param>
        /// <param name="t">Температура в узле</param>
        /// <param name="qin">Расход поступающий в узел</param>
        /// <param name="qout">Расход исходящий из узла</param>
        public override void FinalizeHydraulic(double p, double t, double qin, double qout)
        {
            PressureCalc = p;
            TemperatureCalc = t;
            //double qdiff = qin - qout;
            FlowCalc = qout;
        }
    }
}
