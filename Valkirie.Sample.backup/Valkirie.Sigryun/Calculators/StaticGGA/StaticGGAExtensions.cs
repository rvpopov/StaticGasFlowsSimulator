namespace Valkyrie.Sigryun.Calculators.StaticGGA
{
    /// <summary>
    /// Класс расширений для метода глобального градиента
    /// </summary>
    static class StaticGGAExtensions
    {
        /// <summary>
        /// Возвращает модель объекта, приведенного к интерфейсу IStaticGGAComputationalModel
        /// </summary>
        /// <param name="arc">Дуга расчетного графа</param>
        /// <returns>Модель объекта, приведенная к интерфейсу IStaticGGAComputationalModel</returns>
        public static IStaticGGAComputationalModel CurrentModel(this Graphs.GraphArc arc)
        {
            return arc.Model as IStaticGGAComputationalModel;
        }
    }
}
