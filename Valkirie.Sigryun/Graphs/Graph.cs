using System;
using System.Collections.Generic;
using System.Linq;
using Valkyrie.Sigryun.Interfaces;

namespace Valkyrie.Sigryun.Graphs
{
    /// <summary>
    /// Класс расчетного графа
    /// </summary>
    public class Graph
    {
        #region Приватные переменные

        /// <summary>
        /// Счетчик рантайм идентификаторов
        /// TODO: подумать а нужны ли они вообще?
        /// </summary>
        private static int _runtimeId = 1;

        List<GraphArc> _arcs;   //Дуги графа
        List<GraphNode> _nodes; //Узлы графа

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает расчетный граф
        /// </summary>
        public Graph()
        {
            //Инициализация переменных
            _arcs = new List<GraphArc>();
            _nodes = new List<GraphNode>();
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Узлы графа
        /// </summary>
        public List<GraphNode> Nodes
        {
            get
            {
                return _nodes;
            }
        }

        /// <summary>
        /// Дуги графа
        /// </summary>
        public List<GraphArc> Arcs
        {
            get
            {
                return _arcs;
            }
        }

        #endregion

        #region Методы

        #region Узлы

        /// <summary>
        /// Создает узел в графе
        /// </summary>
        /// <returns>Новый узел</returns>
        public GraphNode AddNode()
        {
            var node = AddNode(_runtimeId++);
            return node;
        }

        /// <summary>
        /// Создает узел в графе
        /// </summary>
        /// <returns>Новый узел</returns>
        public GraphNode AddNode(int id)
        {
            var node = new GraphNode(id, this);
            _nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Возвращает узел по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GraphNode GetNode(int id)
        {
            return _nodes.SingleOrDefault(n => n.Id == id);
        }

        /// <summary>
        /// Удаляет узел из графа
        /// </summary>
        /// <param name="id"></param>
        public void RemoveNode(int id)
        {
            var node = GetNode(id);
            if (node != null)
                RemoveNode(node);
        }

        /// <summary>
        /// Удаляет узел из графа
        /// </summary>
        /// <param name="id"></param>
        public void RemoveNode(GraphNode node)
        {
            RemoveArcs(node.ArcsIn);
            RemoveArcs(node.ArcsOut);
            _nodes.Remove(node);
        }

        #endregion

        #region Дуги

        /// <summary>
        /// Добавляет дугу в граф
        /// </summary>
        /// <param name="beginNode">Начальный узел</param>
        /// <param name="endNode">Конечный узел</param>
        /// <param name="model">Моделируемый объект</param>
        /// <returns>Новая дуга</returns>
        public GraphArc AddArc(GraphNode beginNode, GraphNode endNode, IComputationalModel model)
        {
            var arc = AddArc(_runtimeId++, beginNode, endNode, model);
            return arc;
        }

        /// <summary>
        /// Добавляет дугу в граф
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="beginNode">Начальный узел</param>
        /// <param name="endNode">Конечный узел</param>
        /// <param name="model">Моделируемый объект</param>
        /// <returns>Новая дуга</returns>
        public GraphArc AddArc(int id, GraphNode beginNode, GraphNode endNode, IComputationalModel model)
        {
            var arc = new GraphArc(id, this, beginNode, endNode, model);
            beginNode.ArcsOut.Add(arc);
            endNode.ArcsIn.Add(arc);
            _arcs.Add(arc);
            return arc;
        }

        /// <summary>
        /// Функция возвращает дугу графа
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public GraphArc GetArc(int id)
        {
            return _arcs.SingleOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Удаляет дугу из графа
        /// </summary>
        /// <param name="arc">Дуга</param>
        public void RemoveArc(GraphArc arc)
        {
            if (arc.BeginNode != null)
            {
                arc.BeginNode.ArcsOut.Remove(arc);
            }
            if (arc.EndNode != null)
            {
                arc.EndNode.ArcsIn.Remove(arc);
            }
            _arcs.Remove(arc);
        }

        /// <summary>
        /// Удаляет дуги из графа
        /// </summary>
        /// <param name="arc">Дуги</param>
        public void RemoveArcs(IEnumerable<GraphArc> arcs)
        {
            foreach (var arc in arcs.ToArray())
            {
                RemoveArc(arc);
            }
        }

        #endregion

        #region Другое

        /// <summary>
        /// Функция возвращает матрицу инцидентности
        /// </summary>
        /// <returns></returns>
        public double[,] GetIncedenceMatrix(IEnumerable<GraphNode> sortedNodes)
        {
            double[,] result = new double[sortedNodes.Count(), Arcs.Count()];
            int index = 0;
            var arcsList = Arcs.ToList();
            foreach (var node in sortedNodes)
            {
                for (int i = 0; i < node.ArcsIn.Count(); i++)
                {
                    int numArc = arcsList.IndexOf(node.ArcsIn[i]);
                    result[index, numArc] = -1;
                }
                for (int i = 0; i < node.ArcsOut.Count(); i++)
                {
                    int numArc = arcsList.IndexOf(node.ArcsOut[i]);
                    result[index, numArc] = 1;
                }
                index++;
            }
            return result;
        }

        /// <summary>
        /// Создает копию объекта
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Вызывает эквиваленирование игнорируемых дуг
        /// </summary>
        public void EquivalentIngnorableArcs()
        {
            var arc = _arcs.FirstOrDefault(o => o.Model.IsIgnorable);
            var cycles = _arcs.Where(o => o.BeginNode == o.EndNode);
            while (arc != null)
            {
                if (arc.BeginNode == arc.EndNode)
                {
                    arc.BeginNode.IgnorableModels.Add(arc.Model);
                    RemoveArc(arc);
                    if (arc.BeginNode.ArcsIn.Count + arc.BeginNode.ArcsOut.Count == 0)
                    {

                    }
                }
                else
                {
                    var beginNode = arc.BeginNode;
                    var endNode = arc.EndNode;
                    var model = arc.Model;

                    RemoveArc(arc);

                    var newNode = AddNode();

                    var asd = beginNode.ArcsIn.Union(endNode.ArcsIn)
                        .Intersect(beginNode.ArcsOut.Union(endNode.ArcsOut))
                        .ToArray();

                    beginNode.ArcsIn.Union(endNode.ArcsIn)
                        .ToList()
                        .ForEach(arcIn =>
                        {
                            AddArc(arcIn.BeginNode, newNode, arcIn.Model);
                            RemoveArc(arcIn);
                        });
                    beginNode.ArcsOut.Union(endNode.ArcsOut)
                        .ToList()
                        .ForEach(arcOut =>
                        {
                            AddArc(newNode, arcOut.EndNode, arcOut.Model);
                            RemoveArc(arcOut);
                        });


                    //endNode.ArcsIn
                    //    .ToList()
                    //    .ForEach(arcIn =>
                    //    {
                    //        AddArc(arc.BeginNode, newNode, arcIn.Model);
                    //        RemoveArc(arcIn);
                    //    });
                    //endNode.ArcsOut
                    //    .ToList()
                    //    .ForEach(arcOut =>
                    //    {
                    //        AddArc(newNode, arcOut.EndNode, arcOut.Model);
                    //        RemoveArc(arcOut);
                    //    });

                    newNode.IgnorableModels.Add(arc.Model);
                    newNode.IgnorableModels.AddRange(beginNode.IgnorableModels);
                    newNode.IgnorableModels.AddRange(endNode.IgnorableModels);

                    RemoveNode(beginNode);
                    RemoveNode(endNode);
                }
                arc = _arcs.FirstOrDefault(o => o.Model.IsIgnorable);
            }
        }

        /// <summary>
        /// Производит удаление отключенных объектов графа
        /// </summary>
        public void RemoveDisabledArcs()
        {
            _arcs.Where(o => o.Model.IsDisabled)
                .ToList()
                .ForEach(o => RemoveArc(o));
        }

        public void RemoveDanglingNodes()
        {
            var danglingNode = Nodes.FirstOrDefault(n => n.SignPQ == Enums.NodeBoundarySign.None
                && (n.ArcsIn.Count + n.ArcsOut.Count) == 1 || n.ArcsIn.Count + n.ArcsOut.Count == 0);
            while (danglingNode != null)
            {
                RemoveNode(danglingNode);

                danglingNode = Nodes.FirstOrDefault(n => n.SignPQ == Enums.NodeBoundarySign.None
                    && (n.ArcsIn.Count + n.ArcsOut.Count) == 1 || n.ArcsIn.Count + n.ArcsOut.Count == 0);
            }
        }

        #endregion

        #endregion

        #region Приватные методы

        #endregion
    }
}
