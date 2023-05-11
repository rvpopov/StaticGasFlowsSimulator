
namespace Valkyrie.Sigryun.Graphs
{
    public abstract class GraphObject
    {
        #region Конструктор

        /// <summary>
        /// Создает объект расчетного графа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentGraph"></param>
        public GraphObject(int id, Graph parentGraph)
        {
            Id = id;
            ParentGraph = parentGraph;
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получает идентификатор объекта
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Получает или задает родительский граф
        /// </summary>
        public Graph ParentGraph
        {
            get;
            set;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Функция создает копию объекта
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        #endregion
    }
}
