namespace Valkyrie.Sigryun.Interfaces
{
    public interface IHydraulicNodeFinalize
    {
        /// <summary>
        /// Производит обновление параметров модели по результатам расчета
        /// </summary>
        /// <param name="p">Давление в узле</param>
        /// <param name="t">Температура в узле</param>
        /// <param name="qin">Расход поступающий в узел</param>
        /// <param name="qout">Расход исходящий из узла</param>
        void FinalizeHydraulic(double p, double t, double qin, double qout);
    }
}
