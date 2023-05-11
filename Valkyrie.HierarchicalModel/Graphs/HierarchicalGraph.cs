using System;
using System.Collections.Generic;
using System.Linq;
using Valkyrie.HierarchicalModel.Interfaces;
using Valkyrie.HierarchicalModel.Models;
using Valkyrie.Sigryun;
using Valkyrie.Sigryun.Graphs;

namespace Valkyrie.HierarchicalModel.Graphs
{
    /// <summary>
    /// Представляет иерархический граф
    /// </summary>
    public class HierarchicalGraph
    {
        #region Приватные переменные

        /// <summary>
        /// Словарь дуг
        /// </summary>
        private Dictionary<int, Arc> _arcs;

        /// <summary>
        /// Словарь узлов
        /// </summary>
        private Dictionary<int, Node> _nodes;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает иерархический граф
        /// </summary>
        public HierarchicalGraph()
        {
            _arcs = new Dictionary<int, Arc>();
            _nodes = new Dictionary<int, Node>();
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получает список дуг
        /// </summary>
        public IEnumerable<Arc> Arcs
        {
            get
            {
                return _arcs.Values;
            }
        }

        /// <summary>
        /// Получает список узлов
        /// </summary>
        public IEnumerable<Node> Nodes
        {
            get
            {
                return _nodes.Values;
            }
        }

        /// <summary>
        /// Получает список узлов, связанных с математической моделью
        /// </summary>
        public IEnumerable<ComputationalNode> ComputationalNodes
        {
            get
            {
                return _nodes.Values.OfType<ComputationalNode>().ToArray();
            }
        }

        /// <summary>
        /// Получает список иерархических узлов
        /// </summary>
        public IEnumerable<HierarchicalNode> HierarchicalNodes
        {
            get
            {
                return _nodes.Values.OfType<HierarchicalNode>().ToArray();
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Создает и возвращает узел расчетного графа
        /// </summary>
        /// <returns>Узел расчтного графа</returns>
        public Node CreateNode()
        {
            var node = new Node(this);
            _nodes.Add(node.Id, node);
            return node;
        }

        /// <summary>
        /// Создает и возвращает узел иерархического графа, связанного с математической моделью объекта
        /// </summary>
        /// <param name="model">Математическая модель объекта</param>
        /// <returns>Узел расчтного графа</returns>
        public ComputationalNode CreateComputationalNode(IComputationalModel model)
        {
            var node = new ComputationalNode(this, model);
            _nodes.Add(node.Id, node);
            return node;
        }

        /// <summary>
        /// Создает и возвращает узел иерархического графа, связанного с математической моделью объекта
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Узел расчтного графа</returns>
        public HierarchicalNode CreateHierarchicalNode(IHierarchicalModel model)
        {
            var node = new HierarchicalNode(this, model);
            _nodes.Add(node.Id, node);
            return node;
        }

        /// <summary>
        /// Создает и возвращает дугу расчетного графа
        /// </summary>
        /// <param name="nodeStart">Узел в начале дуги</param>
        /// <param name="nodeEnd">Узел в конце дуги</param>
        /// <returns>Дуга графа</returns>
        public Arc CreateArc(Node nodeStart, Node nodeEnd)
        {
            if (!_nodes.Contains(new KeyValuePair<int, Node>(nodeStart.Id, nodeStart)) ||
                !_nodes.Contains(new KeyValuePair<int, Node>(nodeEnd.Id, nodeEnd)))
                throw new Exception("Попытка связать узлы, отсутсвующие в структуре графа");
            var arc = new Arc(this, nodeStart, nodeEnd);
            nodeStart._outletArcs.Add(arc);
            nodeEnd._inletArcs.Add(arc);
            _arcs.Add(arc.Id, arc);
            return arc;
        }

        /// <summary>
        /// Возвращает узел графа по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор узла</param>
        /// <returns>Узел графа</returns>
        public Node GetNode(int id)
        {
            Node node;
            if (_nodes.TryGetValue(id, out node))
                return node;
            else
                return null;
        }

        /// <summary>
        /// Возвращает дугу графа по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор дуги</param>
        /// <returns>Дуга графа</returns>
        public Arc GetArc(int id)
        {
            Arc arc;
            if (_arcs.TryGetValue(id, out arc))
                return arc;
            else
                return null;
        }

        /// <summary>
        /// Производит удаление дуги из графа
        /// </summary>
        /// <param name="id">Идентификатор дуги</param>
        public void DeleteArc(int id)
        {
            Arc arc;
            if (_arcs.TryGetValue(id, out arc))
                DeleteArc(arc);
            else
                throw new Exception("Попытка удалить дугу, отсутствующую в структуре графа");
        }

        /// <summary>
        /// Производит удаление дуги из графа
        /// </summary>
        /// <param name="arc">Удяляемая дуга</param>
        public void DeleteArc(Arc arc)
        {
            if (arc.ParentGraph != this)
                throw new Exception("Попытка удалить дугу, отсутствующую в структуре графа");
            if (!arc.NodeStart._outletArcs.Remove(arc))
                throw new Exception("Обнаружена ошибка, о которой следует оповестить разработчиков");
            if (!arc.NodeEnd._inletArcs.Remove(arc))
                throw new Exception("Обнаружена ошибка, о которой следует оповестить разработчиков");
        }

        /// <summary>
        /// Производит удаление зула из графа
        /// </summary>
        /// <param name="id">Идентификатор узла</param>
        public void DeleteNode(int id)
        {
            Node node;
            if (_nodes.TryGetValue(id, out node))
                DeleteNode(node);
            else
                throw new Exception("Попытка удалить узел, отсутствующий в структуре графа");
        }

        /// <summary>
        /// Производит удаление узла из графа
        /// </summary>
        /// <param name="arc">Удяляемый узел</param>
        public void DeleteNode(Node node)
        {
            if (node.ParentGraph != this)
                throw new Exception("Попытка удалить узел, отсутствующий в структуре графа");
            foreach (var arc in node.InletArcs.Union(node.OutletArcs).ToArray())
            {
                DeleteArc(arc);
            }
            _nodes.Remove(node.Id);
        }

        /// <summary>
        /// Производит сопоставление расчетного и иерархического графа
        /// </summary>
        /// <param name="graph">Расчетный граф</param>
        public void UpdateHierarchicalComputationalResults(Graph graph)
        {
            var computationalModels = ComputationalNodes.Select(o => o.Model);

            var enters = ComputationalNodes
                .Where(o => o.Model.GetType() == typeof(VirtualEnterModel))
                .ToArray();
            foreach (var virtualEnter in enters)
            {
                var computationalNode = graph.Nodes.SingleOrDefault(o => o.IgnorableModels.Contains(virtualEnter.Model));
                if (computationalNode == null)
                    continue;
                double qin = 0;
                double qout = 0;

                foreach (var arc in computationalNode.ArcsIn.Where(a => a.QBegin > 0)
                    .Union(computationalNode.ArcsOut.Where(a => a.QBegin < 0))
                    .Where(a => computationalModels.Contains(a.Model))
                    .ToArray())
                {
                    qin += Math.Abs(arc.QBegin);
                }

                foreach (var arc in computationalNode.ArcsOut.Where(a => a.QBegin > 0)
                    .Union(computationalNode.ArcsIn.Where(a => a.QBegin < 0))
                    .Where(a => computationalModels.Contains(a.Model))
                    .ToArray())
                {
                    qout += Math.Abs(arc.QBegin);
                }
                (virtualEnter.Model as VirtualEnterModel).FinalizeHydraulic(computationalNode.Pcalc,
                    computationalNode.Tcalc, qin, qout);
            }


            var exits = ComputationalNodes
                .Where(o => o.Model.GetType() == typeof(VirtualExitModel))
                .ToArray();
            foreach (var virtualExit in exits)
            {
                var computationalNode = graph.Nodes.SingleOrDefault(o => o.IgnorableModels.Contains(virtualExit.Model));
                if (computationalNode == null)
                    continue;
                double qin = 0;
                double qout = 0;
                double loos = 0;

                foreach (var arc in computationalNode.ArcsIn.Where(a => a.QBegin > 0)
                    .Union(computationalNode.ArcsOut.Where(a => a.QBegin < 0))
                    .Where(a => computationalModels.Contains(a.Model))
                    .ToArray())
                {
                    qin += Math.Abs(arc.QBegin);
                }

                foreach (var arc in computationalNode.ArcsIn.Where(a => a.QBegin > 0)
                    .Union(computationalNode.ArcsOut.Where(a => a.QBegin < 0))
                    .Where(a => computationalModels.Contains(a.Model))
                    .ToArray())
                {
                    qout += Math.Abs(arc.QBegin);
                    loos += arc.FlowLoos;
                }
                qout += loos;
                (virtualExit.Model as VirtualExitModel).FinalizeHydraulic(computationalNode.Pcalc,
                    computationalNode.Tcalc, qin, qout);
            }

            foreach (var item in HierarchicalNodes)
            {
                item.Model.ChildGraph.UpdateHierarchicalComputationalResults(graph);
            }
        }

        #endregion
    }
}
