using System;
using System.Collections.Generic;
using System.Linq;
using Valkyrie.Sigryun.Graphs;
using Valkyrie.Sigryun.Interfaces;

namespace Valkyrie.Sigryun.Calculators.TermalCalculator
{
    /// <summary>
    /// Класс для расчета тепературы в ГТС
    /// </summary>
    public class TermalCalculator
    {
        #region Приватные переменные

        /// <summary>
        /// Расчетный граф
        /// </summary>
        private Graph _graph;

        /// <summary>
        /// Узлы, для которых температура определена
        /// </summary>
        private List<GraphNode> _calculatedNodes;

        /// <summary>
        /// Список текущих входных узлов
        /// </summary>
        private List<GraphNode> _currentNodeLayer;

        /// <summary>
        /// Список следующих выходных узлов
        /// </summary>
        private List<GraphNode> _nextNodeLayer;

        /// <summary>
        /// Узлы смешивания потоков
        /// </summary>
        private Dictionary<GraphNode, List<double[]>> _mixflowNodes;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает эксземпляр класса для расчета тепературы в ГТС
        /// </summary>
        public TermalCalculator()
        {

        }

        #endregion

        #region Свойства

        /// <summary>
        /// Расчетный граф
        /// </summary>
        public Graph Graph
        {
            get
            {
                return _graph;
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Производит расчет температуры в ГТС
        /// </summary>
        /// <param name="graph">Расчетный граф</param>
        public void Calculate(Graph graph)
        {
            //Инициализируем объект
            _graph = graph;
            _calculatedNodes = new List<GraphNode>();
            _currentNodeLayer = new List<GraphNode>();
            _nextNodeLayer = new List<GraphNode>();
            _mixflowNodes = new Dictionary<GraphNode, List<double[]>>();
            //Получаем узлы с задаными давлениями
            var inputlNodes = graph.Nodes.Where(o => o.T > 0 && o.Qcalc >= 0).ToArray();
            inputlNodes.ToList().ForEach(n => n.Tcalc = n.T);
            _currentNodeLayer.AddRange(inputlNodes);
            _calculatedNodes.AddRange(inputlNodes);
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                CalculateNodesLayer(_currentNodeLayer);
                if (graph.Nodes.Count == _calculatedNodes.Count)
                    break;
                _currentNodeLayer.Clear();
                _currentNodeLayer.AddRange(_nextNodeLayer);
                _nextNodeLayer.Clear();
            }

        }

        #endregion

        #region Приватные методы

        public void CalculateNodesLayer(List<GraphNode> nodes)
        {
            foreach (var node in nodes)
            {
                CalculateNode(node);
            }
            List<GraphNode> toremove = new List<GraphNode>();
            foreach (var item in _mixflowNodes)
            {
                if (GetArcsIn(item.Key).Length != item.Value.Count)
                    continue;
                double qsum = item.Value.Select(o => o[0]).Sum();
                double tavg = item.Value.Select(o => o[0] * o[1]).Sum() / qsum;
                item.Key.Tcalc = tavg;
                _calculatedNodes.Add(item.Key);
                if (GetArcsOut(item.Key).Length > 0)
                    _nextNodeLayer.Add(item.Key);
                if (item.Key.IgnorableModels.Any(o => o is ITemperatureSetpoint))
                    item.Key.Tcalc = (item.Key.IgnorableModels.First(o => o is ITemperatureSetpoint) as ITemperatureSetpoint).TemperatureSetpoint;

                toremove.Add(item.Key);
            }
            foreach (var item in toremove)
            {
                _mixflowNodes.Remove(item);
            }
        }

        public void CalculateNode(GraphNode node)
        {
            var arcsOut = GetArcsOut(node);
            foreach (var arc in arcsOut)
            {
                //Определяем конечный узел
                GraphNode beginNode = (arc.QBegin >= 0) ? arc.BeginNode : arc.EndNode;
                GraphNode endNode = (arc.QBegin >= 0) ? arc.EndNode : arc.BeginNode;
                if (_calculatedNodes.Contains(endNode))
                    continue;
                //throw new Exception("Попытка расчета температуры в рассчитанном узле");
                double tend = arc.CurrentModel().CalculateFinishTemperature(arc.BeginNode.Pcalc, arc.EndNode.Pcalc, arc.QBegin, arc.QEnd, beginNode.Tcalc);
                arc.TBegin = (arc.QBegin >= 0) ? beginNode.Tcalc : tend;
                arc.TEnd = (arc.QBegin >= 0) ? tend : beginNode.Tcalc;

                var arcsIn = GetArcsIn(endNode);
                if (arcsIn.Length == 1)
                {
                    endNode.Tcalc = tend;

                    if (endNode.IgnorableModels.Any(o => o is ITemperatureSetpoint))
                        endNode.Tcalc = (endNode.IgnorableModels.First(o => o is ITemperatureSetpoint) as ITemperatureSetpoint).TemperatureSetpoint;


                    _calculatedNodes.Add(endNode);
                    CalculateNode(endNode);
                }
                else
                {
                    if (!_mixflowNodes.ContainsKey(endNode))
                        _mixflowNodes.Add(endNode, new List<double[]>());
                    _mixflowNodes[endNode].Add(new double[] { Math.Abs(arc.QEnd), tend });
                }
            }
        }

        /// <summary>
        /// Функция возвращает исходящие дуги, относительно направления потока газа
        /// </summary>
        /// <param name="node">Узел расчетного графа</param>
        /// <returns>Исходящие дуги, относительно потока газа</returns>
        protected GraphArc[] GetArcsOut(GraphNode node)
        {
            return node.ArcsIn.Where(o => o.QEnd < 0).Union(node.ArcsOut.Where(o => o.QBegin >= 0)).ToArray();
        }

        /// <summary>
        /// Функция возвращает заходящие дуги, относительного направления потока газа
        /// </summary>
        /// <param name="node">Узел расчетного графа</param>
        /// <returns>Заходящие дуги, относительно потока газа</returns>
        protected GraphArc[] GetArcsIn(GraphNode node)
        {
            return node.ArcsIn.Where(o => o.QEnd >= 0).Union(node.ArcsOut.Where(o => o.QBegin < 0)).ToArray();
        }

        #endregion
    }
}
