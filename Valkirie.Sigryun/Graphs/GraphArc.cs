using System;

namespace Valkyrie.Sigryun.Graphs
{
    /// <summary>
    /// Представляет дугу расчетного графа
    /// </summary>
    public class GraphArc : GraphObject
    {
        #region Конструктор

        /// <summary>
        /// Создает дугу расчетного графа
        /// </summary>
        /// <param name="id">Идентификатор дуги</param>
        /// <param name="parentGraph">Родительский граф</param>
        /// <param name="beginNode">Узел начала</param>
        /// <param name="endNode">Узел конца</param>
        /// <param name="model">Модель объекта</param>
        internal GraphArc(int id, Graph parentGraph, GraphNode beginNode, GraphNode endNode, IComputationalModel model)
            : base(id, parentGraph)
        {
            BeginNode = beginNode;
            EndNode = endNode;
            Model = model;
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получает или задает узел начала
        /// </summary>
        public GraphNode BeginNode
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает узел окончания
        /// </summary>
        public GraphNode EndNode
        {
            get;
            set;
        }

        /// <summary>
        /// Получает моделируемый объект
        /// </summary>
        public IComputationalModel Model
        {
            get;
            private set;
        }

        /// <summary>
        /// Расход в начале дуги
        /// </summary>
        public double QBegin
        {
            get;
            set;
        }

        /// <summary>
        /// Расход в конце дуги
        /// </summary>
        public double QEnd
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или  задает потери газа на дуге
        /// </summary>
        public double FlowLoos
        {
            get;
            set;
        }

        /// <summary>
        /// Температура в начале дуги
        /// </summary>
        public double TBegin
        {
            get;
            set;
        }

        /// <summary>
        /// Температура в конце дуги
        /// </summary>
        public double TEnd
        {
            get;
            set;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Функция устанавливает узлы начала и конца дуги
        /// Неосторожное использование использование этого метода может привести к нарушению целостности структуры графа
        /// </summary>
        /// <param name="beginNode">Узел начала</param>
        /// <param name="endNode">Узел конца</param>
        public void SetNodes(GraphNode beginNode, GraphNode endNode)
        {
            this.BeginNode = beginNode;
            this.EndNode = endNode;
            beginNode.ArcsOut.Add(this);
            endNode.ArcsIn.Add(this);
        }

        /// <summary>
        /// Функция создает копию объекта
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
