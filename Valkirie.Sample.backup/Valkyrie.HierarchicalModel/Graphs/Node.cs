using System.Collections.Generic;

namespace Valkyrie.HierarchicalModel.Graphs
{
    /// <summary>
    /// Представляет узел иерархического графа
    /// </summary>
    public class Node : GraphObjectBase
    {
        #region Приватные перемнные

        /// <summary>
        /// Список заходящих в узел дуг
        /// </summary>
        internal List<Arc> _inletArcs;

        /// <summary>
        /// Список исходящих из узла дуг
        /// </summary>
        internal List<Arc> _outletArcs;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает узел иерархического графа
        /// </summary>
        /// <param name="gprah">Родительский граф</param>
        internal Node(HierarchicalGraph graph)
            : base(graph)
        {
            _inletArcs = new List<Arc>();
            _outletArcs = new List<Arc>();
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получает список входящих в узел дуг
        /// </summary>
        public IEnumerable<Arc> InletArcs
        {
            get
            {
                return _inletArcs;
            }
        }

        /// <summary>
        /// Получает список исходящих из узла дуг
        /// </summary>
        public IEnumerable<Arc> OutletArcs
        {
            get
            {
                return _outletArcs;
            }
        }

        #endregion
    }
}
