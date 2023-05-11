using System;
using System.Linq;
using Valkyrie.HierarchicalModel.Graphs;
using Valkyrie.HierarchicalModel.Models;
using Valkyrie.Sigryun.Enums;
using Valkyrie.Sigryun.Graphs;

namespace Valkyrie.HierarchicalModel.Helpers
{
    public class HierarchicalModelConverter
    {
        #region Приватные переменные

        Graph _resultGraph;

        #endregion

        #region Методы

        public Graph CreateComputationalGraph(HierarchicalGraph graph)
        {
            _resultGraph = new Graph();

            ConvertGraph(graph);

            return _resultGraph;
        }

        public void CreateFlowSources(Graph graph)
        {
            _resultGraph = graph;
            if (_resultGraph.Arcs.Any(o => o.Model is FlowSourceBase))
                throw new Exception("Попытка соформировать граничные условия для графа с не эквивалентированными объектами");
            var sourceNodes = _resultGraph.Nodes
                .Where(o => o.IgnorableModels.Any(a => a is EnterModel || a is ExitModel))
                .ToArray();
            if (sourceNodes.Length == 0)
                throw new Exception("В структуре расчетного графа не обнаружены поставщики и потрбители");
            foreach (var source in sourceNodes)
            {
                var enterModels = source.IgnorableModels
                    .OfType<EnterModel>()
                    .ToArray();
                var exitModels = source.IgnorableModels
                    .OfType<ExitModel>()
                    .ToArray();

                //Проверка на то, что все поставщики и потребители должны иметь один знак
                var singns = enterModels.Select(o => o.BoundarySign)
                    .Union(exitModels.Select(o => o.BoundarySign))
                    .Distinct();
                if (enterModels.Cast<FlowSourceBase>().Union(exitModels).Count() > 1 && !singns.All(o => o == NodeBoundarySign.Flow))
                    throw new Exception("В узле " + enterModels.Cast<FlowSourceBase>().Union(exitModels).First().Tag
                        + " заданы граничные условия разных типов");
                source.Q = 0;
                if (enterModels.Count() > 0)
                {
                    if (enterModels.First().BoundarySign == NodeBoundarySign.Pressure)
                    {
                        source.SignPQ = NodeBoundarySign.Pressure;
                        source.P = enterModels.First().Pressure;
                        source.T = enterModels.First().Temperature;
                    }
                    else if (enterModels.All(o => o.BoundarySign == NodeBoundarySign.Flow))
                    {
                        source.SignPQ = NodeBoundarySign.Flow;
                        source.Q += enterModels.Sum(o => o.Flow);
                        source.T = enterModels.Select(o => o.Temperature * o.Flow / source.Q).Sum();
                    }
                    else
                    {
                        throw new Exception("В структуре графа есть поставщик с незаданным граничным условием");
                    }
                }
                if (exitModels.Count() > 0)
                {
                    if (exitModels.First().BoundarySign == NodeBoundarySign.Pressure)
                    {
                        source.SignPQ = NodeBoundarySign.Pressure;
                        source.P = exitModels.First().Pressure;
                    }
                    else if (exitModels.All(o => o.BoundarySign == NodeBoundarySign.Flow))
                    {
                        source.SignPQ = NodeBoundarySign.Flow;
                        source.Q -= exitModels.Sum(o => o.Flow);
                    }
                    else
                    {
                        throw new Exception("В структуре графа есть поставщик с незаданным граничным условием");
                    }
                }
            }
        }

        #endregion

        #region Приватные методы

        private void ConvertGraph(HierarchicalGraph graph)
        {
            var computationalNodes = graph.ComputationalNodes.ToArray();
            foreach (var node in computationalNodes)
            {
                if (!(node.InletArcs.Count() == 1 || node.OutletArcs.Count() == 1))
                    throw new Exception("Для расчетного узла " + node.Model.Tag + " не правильно сформированы коннекторы");

                if (node.InletArcs.Count() == 1 && node.OutletArcs.Count() == 0)
                {
                    var nodeStart = _resultGraph.GetNode(node.InletArcs.Single().NodeStart.Id)
                        ?? _resultGraph.AddNode(node.InletArcs.Single().NodeStart.Id);
                    nodeStart.IgnorableModels.Add(node.Model);
                }
                else if (node.InletArcs.Count() == 0 && node.OutletArcs.Count() == 1)
                {
                    var nodeFinish = _resultGraph.GetNode(node.OutletArcs.Single().NodeEnd.Id)
                        ?? _resultGraph.AddNode(node.OutletArcs.Single().NodeEnd.Id);
                    nodeFinish.IgnorableModels.Add(node.Model);
                }
                else
                {
                    var nodeStart = _resultGraph.GetNode(node.InletArcs.Single().NodeStart.Id)
                        ?? _resultGraph.AddNode(node.InletArcs.Single().NodeStart.Id);
                    var nodeFinish = _resultGraph.GetNode(node.OutletArcs.Single().NodeEnd.Id)
                        ?? _resultGraph.AddNode(node.OutletArcs.Single().NodeEnd.Id);

                    _resultGraph.AddArc(node.Id, nodeStart, nodeFinish, node.Model);
                }
            }

            foreach (var node in graph.HierarchicalNodes)
            {
                ConvertGraph(node.Model.ChildGraph);
                foreach (var link in node.HierarchicalLinks)
                {
                    var model = (link.ChildLinkedNode as ComputationalNode).Model;
                    var borderNode = _resultGraph.Nodes
                        .SingleOrDefault(n => n.IgnorableModels.Contains(model));
                    if (borderNode == null)
                        throw new Exception("Ошибка при поиске внешнего узла на дочернем графе");
                    if (node.InletArcs.Contains(link.ParentArcConnector))
                    {
                        var startNode = _resultGraph.GetNode(link.ParentArcConnector.NodeStart.Id);
                        _resultGraph.AddArc(startNode, borderNode, model);
                        borderNode.IgnorableModels.Remove(model);
                    }
                    else if (node.OutletArcs.Contains(link.ParentArcConnector))
                    {
                        var finishNode = _resultGraph.GetNode(link.ParentArcConnector.NodeEnd.Id);
                        _resultGraph.AddArc(borderNode, finishNode, model);
                        borderNode.IgnorableModels.Remove(model);
                    }
                    else
                    {
                        throw new Exception("Коннектор связи узла невозможно сопоставить с инцидентными дугами");
                    }
                }
            }
        }


        #endregion
    }
}
