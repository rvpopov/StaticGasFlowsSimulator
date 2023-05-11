namespace Valkyrie.Sigryun.Calculators.TermalCalculator
{
    static class TermalCalculatorExtension
    {
        /// <summary>
        /// Возвращает модель объекта, приведенного к интерфейсу IStaticGGAComputationalModel
        /// </summary>
        /// <param name="arc">Дуга расчетного графа</param>
        /// <returns>Модель объекта, приведенная к интерфейсу IStaticGGAComputationalModel</returns>
        public static ITermalComputationalModel CurrentModel(this Graphs.GraphArc arc)
        {
            return arc.Model as ITermalComputationalModel;
        }
    }
}
