namespace Valkyrie.HierarchicalModel.Graphs
{
    /// <summary>
    /// Представляет связь иерархического узла с его дочерним графом
    /// </summary>
    public class HierarchicalLink
    {
        /// <summary>
        /// Создает связь иерархического узла с его дочерним графом
        /// </summary>
        /// <param name="parentNode">Родительский иерархический узел</param>
        /// <param name="parentArcConnector">Родительская дуга, подключаемая к иерархическому объекту</param>
        /// <param name="childLinkedNode">Соединительный узел на дочернем графе</param>
        internal HierarchicalLink(HierarchicalNode parentNode, Arc parentArcConnector, Node childLinkedNode)
        {
            ParentNode = parentNode;
            ParentArcConnector = parentArcConnector;
            ChildLinkedNode = childLinkedNode;
        }

        /// <summary>
        /// Получает родительский иерархический узел
        /// </summary>
        public HierarchicalNode ParentNode
        {
            get;
            private set;
        }

        /// <summary>
        /// Получает родительскую дугу, подключаемую к иерархическому объекту
        /// </summary>
        public Arc ParentArcConnector
        {
            get;
            private set;
        }

        /// <summary>
        /// Получает соединительный узел на дочернем графе
        /// </summary>
        public Node ChildLinkedNode
        {
            get;
            private set;
        }
    }
}
