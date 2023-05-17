using Valkyrie.HierarchicalModel.Graphs;

namespace Valkyrie.HierarchicalModel.Interfaces
{
    /// <summary>
    /// Интерфейс модели объекта, предсталяемой в виде иерархической сущности
    /// </summary>
    public interface IHierarchicalModel
    {
        /// <summary>
        /// Получает или задает тег объекта
        /// </summary>
        object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Получает дочерний граф иерархического объекта
        /// </summary>
        HierarchicalGraph ChildGraph
        {
            get;
        }
    }
}
