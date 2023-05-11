using System;
using System.Collections.Generic;
using System.Linq;
using Valkyrie.HierarchicalModel.Interfaces;

namespace Valkyrie.HierarchicalModel.Graphs
{
    public class HierarchicalNode : Node
    {
        #region Приватные переменные

        /// <summary>
        /// Список иерархических связей
        /// </summary>
        internal List<HierarchicalLink> _hierarchicalLinks;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает узел иерархического графа, связанного с математической моделью объекта
        /// </summary>
        /// <param name="graph">Родительский граф</param>
        /// <param name="model">Математическая модель объекта</param>
        internal HierarchicalNode(HierarchicalGraph graph, IHierarchicalModel model)
            : base(graph)
        {
            Model = model;
            _hierarchicalLinks = new List<HierarchicalLink>();
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получает математическую модель объекта
        /// </summary>
        public IHierarchicalModel Model
        {
            get;
            private set;
        }

        /// <summary>
        /// Получает список иерархических связей
        /// </summary>
        public IEnumerable<HierarchicalLink> HierarchicalLinks
        {
            get
            {
                return _hierarchicalLinks;
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Создает иерархическую связь с данным узлом
        /// </summary>
        /// <param name="parentArcConnector">Родительская дуга, подключаемая к иерархическому объекту</param>
        /// <param name="childLinkedNode">Соединительный узел на дочернем графе</param>
        /// <returns>Иерархическая связь</returns>
        public HierarchicalLink CreateHierarchicalLink(Arc parentArcConnector, Node childLinkedNode)
        {
            if (!InletArcs.Union(OutletArcs).Contains(parentArcConnector))
                throw new Exception("Попытка создать иерархическую связь, где у узла отсутствуюет соответствующий коннектор");
            if (Model.ChildGraph.GetNode(childLinkedNode.Id) != childLinkedNode)
                throw new Exception("Попытка создать иерархическую связь, где у узла в дочернем графе отсутствуюет соответствующий узел");
            var link = new HierarchicalLink(this, parentArcConnector, childLinkedNode);
            _hierarchicalLinks.Add(link);
            return link;
        }

        /// <summary>
        /// Производит удаление иерархической связи
        /// </summary>
        /// <param name="link">Иерархическая связь</param>
        public void RemoveHierarchicalLink(HierarchicalLink link)
        {
            _hierarchicalLinks.Remove(link);
        }

        #endregion
    }
}
