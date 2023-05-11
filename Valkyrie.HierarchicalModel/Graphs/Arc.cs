namespace Valkyrie.HierarchicalModel.Graphs
{
    /// <summary>
    /// Представляет дугу иерархического графа
    /// </summary>
    public class Arc : GraphObjectBase
    {
        #region Конструктор

        /// <summary>
        /// Создает дугу иерархического графа
        /// </summary>
        /// <param name="gprah">Родительский граф</param>
        internal Arc(HierarchicalGraph graph, Node nodeStart, Node nodeEnd)
            : base(graph)
        {
            NodeStart = nodeStart;
            NodeEnd = nodeEnd;
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получает узел в начале дуги
        /// </summary>
        public Node NodeStart
        {
            get;
            private set;
        }

        /// <summary>
        /// Получает узел в конце дуги
        /// </summary>
        public Node NodeEnd
        {
            get;
            private set;
        }

        #endregion
    }
}
