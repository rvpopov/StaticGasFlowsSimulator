namespace Valkyrie.Sigryun.Interfaces
{
    public interface IHydraulicArcFinalize
    {
        /// <summary>
        /// Производит обновление параметров модели по результатам расчета
        /// </summary>
        /// <param name="pin">Давление в начале дуги</param>
        /// <param name="pout">Давление в конце дуги</param>
        /// <param name="qin">Расход на входе дуги</param>
        void FinalizeHydraulic(double pin, double pout, double qin);
    }
}
