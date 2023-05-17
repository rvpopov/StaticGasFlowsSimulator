using Valkyrie.Sigryun;

namespace Valkyrie.HierarchicalModel.Graphs
{
    /// <summary>
    /// Представляет узел иерархического графа, связанного с математической моделью
    /// </summary>
    public class ComputationalNode : Node
    {
        #region Конструктор

        /// <summary>
        /// Создает узел иерархического графа, связанного с математической моделью объекта
        /// </summary>
        /// <param name="graph">Родительский граф</param>
        /// <param name="model">Математическая модель объекта</param>
        internal ComputationalNode(HierarchicalGraph graph, IComputationalModel model)
            : base(graph)
        {
            Model = model;
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получает математическую модель объекта
        /// </summary>
        public IComputationalModel Model
        {
            get;
            private set;
        }

        #endregion

    }
}
