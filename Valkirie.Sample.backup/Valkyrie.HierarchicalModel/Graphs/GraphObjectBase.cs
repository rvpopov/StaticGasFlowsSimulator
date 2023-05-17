namespace Valkyrie.HierarchicalModel.Graphs
{
    /// <summary>
    /// Представляет абстрактный класс объекта иерархического графа
    /// </summary>
    public abstract class GraphObjectBase
    {
        /// <summary>
        /// Инициализация базового объекта иерархического графа
        /// </summary>
        /// <param name="graph">Родительский граф</param>
        internal GraphObjectBase(HierarchicalGraph graph)
        {
            Id = IdentificatorManager.GetNextValue();
            ParentGraph = graph;
        }

        /// <summary>
        /// Получает уникальный идентификатор объекта иерархического графа
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Получает родительский для дуги граф
        /// </summary>
        public HierarchicalGraph ParentGraph
        {
            get;
            private set;
        }
    }
}
