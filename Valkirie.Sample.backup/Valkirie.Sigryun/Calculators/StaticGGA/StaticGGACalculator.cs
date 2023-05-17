using System;
using System.Collections.Generic;
using System.Linq;
using Valkyrie.MathLibrary;
using Valkyrie.Sigryun.Enums;
using Valkyrie.Sigryun.Graphs;
using Valkyrie.Sigryun.Interfaces;

namespace Valkyrie.Sigryun.Calculators.StaticGGA
{
    public class StaticGGACalculator
    {
        #region Приватные переменные

        //Узлы с заданными расходами
        protected List<GraphNode> _nodesQ;
        //Узлы с заданными давлениями
        protected List<GraphNode> _nodesP;

        //Листы узлов и дуг - для ускорения поиска индекса элемента
        protected List<GraphNode> _allNodes;
        protected List<GraphArc> _allArcs;

        //Значения давлений в узлах
        protected double[] _p_nodes;
        //Значения расходов в узлах
        protected double[] _q_nodes;

        //Расчетные расходы на дугах графа
        protected double[] _flows;
        //Матрица Максвелла
        protected double[,] _jacobian;
        protected double[] _rightSides;

        protected double[] _derivatives;
        protected double[] _pressureLooses;
        protected double[] _pressureDisbalance;

        //Алгоритм Холецкого
        protected LUDecompositionMethod _linearSolver;

        /// <summary>
        /// Расчетный граф
        /// </summary>
        protected Graph _graph;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса гидравлического расчета
        /// </summary>
        public StaticGGACalculator()
        {
            MaximumIterations = 100;
            FlowAccuracy = 0.0001;
            PressureAccuracy = 0.0001;
        }

        #endregion

        #region Методы

        public void GenerateStartConditions(Graph graph)
        {
            var pressureMin = graph.Nodes.Where(n => n.SignPQ == Enums.NodeBoundarySign.Pressure).Min(o => o.P);
            var pressureMax = graph.Nodes.Where(n => n.SignPQ == Enums.NodeBoundarySign.Pressure).Max(o => o.P);
            //Random rnd = new Random();
            foreach (var node in graph.Nodes)
            {
                if (node.SignPQ == NodeBoundarySign.Pressure)
                {
                    node.Pcalc = node.P;
                }
                else
                {
                    node.Pcalc = 5;
                }
            }
            for (int i = 0; i < graph.Arcs.Count; i++)
            {
                var arc = graph.Arcs[i];
                arc.QBegin = arc.QEnd = arc.CurrentModel().CalculateInitialFlow(arc.BeginNode.Pcalc, arc.EndNode.Pcalc);
                arc.FlowLoos = arc.CurrentModel().CalculateFlowLoss(arc.BeginNode.Pcalc, arc.EndNode.Pcalc, arc.QBegin);
                arc.EndNode.Qloss += arc.FlowLoos;
            }
        }

        /// <summary>
        /// Производит расчет температуры в ГТС
        /// </summary>
        /// <param name="graph">Расчетный граф</param>
        public virtual void Calculate(Graph graph)
        {
            _graph = graph;
            InitializeCalculator();

            CalculateHydraulics();
        }

        #endregion

        #region Свойства

        #region Настрйоки расчета

        /// <summary>
        /// Получает или задает максимальное количество итерация расчета
        /// </summary>
        public int MaximumIterations
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает точность по давлению, МПа(а)
        /// </summary>
        public double PressureAccuracy
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает точность по расходу, млн.м3/сут (СТУ)
        /// </summary>
        public double FlowAccuracy
        {
            get;
            set;
        }

        #endregion

        public int Iterations
        {
            get;
            protected set;
        }

        public double ErrorQ
        {
            get;
            protected set;
        }

        public double ErrorP
        {
            get;
            protected set;
        }

        public List<GraphArc> ArcsWithDisbalance
        {
            get;
            protected set;
        }

        public Graph Graph
        {
            get
            {
                return _graph;
            }
        }

        #endregion

        #region Приватные методы

        protected virtual void InitializeCalculator()
        {
            //Узлы с заданными расходами
            _nodesQ = Graph.Nodes.Where(o => o.SignPQ != Enums.NodeBoundarySign.Pressure).ToList();
            //Узлы с заданными давлениями
            _nodesP = Graph.Nodes.Where(o => o.SignPQ == Enums.NodeBoundarySign.Pressure).ToList();

            //Листы узлов и дуг - для ускорения поиска индекса элемента
            _allNodes = _nodesQ.Union(_nodesP).ToList();
            _allArcs = Graph.Arcs.ToList();

            //Значения давлений в узлах
            _p_nodes = _allNodes.Select(o => Math.Pow(o.Pcalc, 2)).ToArray();
            //Значения расходов в узлах
            _q_nodes = _allNodes.Select(o => o.Q).ToArray();

            //Расчетные расходы на дугах графа
            _flows = new double[Graph.Arcs.Count];

            for (int i = 0; i < Graph.Arcs.Count; i++)
            {
                var arc = Graph.Arcs[i];
                //_flows[i] = arc.CurrentModel().CalculateInitialFlow(arc.BeginNode.Pcalc, arc.EndNode.Pcalc);
                _flows[i] = (arc.QBegin + arc.QEnd) / 2;
            }
            //Матрица Максвелла
            _jacobian = new double[_nodesQ.Count, _nodesQ.Count];
            _rightSides = new double[_nodesQ.Count];

            _derivatives = new double[Graph.Arcs.Count];
            _pressureLooses = new double[Graph.Arcs.Count];
            _pressureDisbalance = new double[Graph.Arcs.Count];

            //Решение метода Холецкого
            _linearSolver = new LUDecompositionMethod(_nodesQ.Count);

            ArcsWithDisbalance = new List<GraphArc>();
        }

        protected void CalculateHydraulics()
        {
            bool normalResult = false;
            for (int k = 0; k < MaximumIterations; k++)
            {
                double disbP = 0;// dP.Max(o => Math.Sqrt(Math.Abs(o)));
                int disbPIndex = 0;

                //Расчет производных
                CalculateDerivatives();

                //Вычисляем потери давления
                CalculatePressureLooses();
                if (_nodesQ.Count > 0)
                {
                    //Формируем матрицу Максвелла
                    CalculateMaxwellMatrix();
                    //Вычисляем потери давления
                    CalculatePressureDisbalance();
                    //Вычисляем правую сторону системы уравнений
                    CalculateRightSides();

                    //Решаем систему алегебраических уравнений
                    var dP = _linearSolver.Calculate(_jacobian, _rightSides);
                    for (int i = 0; i < _nodesQ.Count; i++)
                    {
                        _p_nodes[i] += dP[i];
                        if (_p_nodes[i] < 2)
                            _p_nodes[i] = 2;
                    }
                    if (double.IsNaN(dP.Sum()))
                    {
                        return;
                    }
                    for (int i = 0; i < dP.Length; i++)
                    {
                        if (Math.Sqrt(Math.Abs(dP[i])) > disbP)
                        {
                            disbP = Math.Sqrt(Math.Abs(dP[i]));
                            disbPIndex = i;
                        }
                    }
                }
                //Обновляем невязку по давлению
                CalculatePressureDisbalance();
                for (int i = 0; i < Graph.Arcs.Count; i++)
                {
                    double qmin, qmax;
                    var arc = Graph.Arcs[i];
                    arc.CurrentModel().CalculateFlowBorders(
                        arc.BeginNode.Pcalc, arc.EndNode.Pcalc,
                        out qmin, out qmax);

                    var qnew = _flows[i] + _pressureDisbalance[i];
                    if (qnew < qmin)
                        qnew = qmin;
                    if (qnew > qmax)
                        qnew = qmax;

                    _flows[i] = qnew;
                }

                double disbQ = _pressureDisbalance.Max(o => Math.Abs(o));

                ArcsWithDisbalance.Clear();
                ArcsWithDisbalance.AddRange(_allNodes[disbPIndex].ArcsIn.Union(_allNodes[disbPIndex].ArcsOut));

                CalculateDisbalance();
                CalculateFlowsInNodes();
                //Console.WriteLine("{0}. Dmax = {1:0.0000000000} Dpsx = {2:0.0000000000}", k, disbQ, disbP);
                if (disbP < PressureAccuracy && disbQ < FlowAccuracy)
                {
                    //Console.WriteLine("{0}. Dmax = {1:0.0000000000} Dpsx = {2:0.0000000000}", k, disbQ, disbP);
                    //Console.WriteLine("Количество итераций расчета гидравлики: " + (k + 1));
                    normalResult = true;
                    break;
                }
                Iterations = k;
                ErrorP = disbP;
                ErrorQ = disbQ;
            }
            if (!normalResult)
                Console.WriteLine("Не удалось сбалансировать режим");
            for (int i = 0; i < _flows.Length; i++)
            {
                Graph.Arcs[i].QBegin = _flows[i];
                Graph.Arcs[i].QEnd = Graph.Arcs[i].QBegin + Graph.Arcs[i].FlowLoos;
                Graph.Arcs[i].CurrentModel().FinalizeHydraulic(Graph.Arcs[i].BeginNode.Pcalc,
                    Graph.Arcs[i].EndNode.Pcalc, _flows[i]);
            }
            foreach (var node in Graph.Nodes)
            {
                double qin = 0;
                double qout = 0;

                foreach (var arc in node.ArcsIn.Where(a => a.QBegin > 0).Union(node.ArcsOut.Where(a => a.QBegin < 0)))
                {
                    qin += Math.Abs(arc.QBegin);
                }

                foreach (var arc in node.ArcsOut.Where(a => a.QBegin > 0).Union(node.ArcsIn.Where(a => a.QBegin < 0)))
                {
                    qout += Math.Abs(arc.QBegin);
                }

                node.Qcalc = qout + node.Qloss - qin;

                foreach (var model in node.IgnorableModels.OfType<IHydraulicNodeFinalize>())
                {
                    model.FinalizeHydraulic(node.Pcalc, node.Tcalc, qin, qout);
                }
            }
        }

        protected void CalculateDerivatives()
        {
            double dq = 0.05;
            for (int i = 0; i < Graph.Arcs.Count; i++)
            {
                var arc = Graph.Arcs[i];
                _derivatives[i] = arc.CurrentModel()
                    .CalculatePressureLossDifferential(arc.BeginNode.Pcalc, arc.EndNode.Pcalc, _flows[i]);

                if (double.IsNaN(_derivatives[i]) || double.IsInfinity(_derivatives[i]) || _derivatives[i] == 0)
                {
                }
            }
            if (double.IsNaN(_derivatives.Sum()))
            {
                return;
                throw new Exception();
            }
        }

        protected void CalculatePressureLooses()
        {
            for (int i = 0; i < Graph.Arcs.Count; i++)
            {
                var arc = Graph.Arcs[i];
                _pressureLooses[i] = arc.CurrentModel()
                    .CalculatePressureLoss(arc.BeginNode.Pcalc, arc.EndNode.Pcalc, _flows[i]);
            }
        }

        protected virtual void CalculateMaxwellMatrix()
        {
            Array.Clear(_jacobian, 0, _nodesQ.Count * _nodesQ.Count);

            for (int i = 0; i < _nodesQ.Count; i++)
            {
                foreach (var arc in _nodesQ[i].ArcsOut)
                {
                    int arc_index = _allArcs.IndexOf(arc);
                    double d = 1 / _derivatives[arc_index];
                    _jacobian[i, i] += d;

                    var nodeEnd = arc.EndNode;
                    int node_index = _nodesQ.IndexOf(nodeEnd);
                    if (node_index == -1)
                        continue;
                    _jacobian[i, node_index] -= d;
                }

                foreach (var arc in _nodesQ[i].ArcsIn)
                {
                    int arc_index = _allArcs.IndexOf(arc);
                    double d = 1 / _derivatives[arc_index];
                    _jacobian[i, i] += d;

                    var nodeBegin = arc.BeginNode;
                    int node_index = _nodesQ.IndexOf(nodeBegin);
                    if (node_index == -1)
                        continue;
                    _jacobian[i, node_index] -= d;
                }
            }
        }

        protected void CalculateRightSides()
        {
            Array.Clear(_rightSides, 0, _nodesQ.Count);
            for (int i = 0; i < _nodesQ.Count; i++)
            {
                var node = _nodesQ[i];
                _rightSides[i] = node.Q + node.Qloss;
                foreach (var arc in node.ArcsOut)
                {
                    int arc_index = _allArcs.IndexOf(arc);
                    _rightSides[i] -= _flows[arc_index];
                    _rightSides[i] -= _pressureDisbalance[arc_index];
                }
                foreach (var arc in node.ArcsIn)
                {
                    int arc_index = _allArcs.IndexOf(arc);
                    _rightSides[i] += _flows[arc_index];
                    _rightSides[i] += _pressureDisbalance[arc_index];
                }
                //if (_rightSides[i] == 0)
                //    _rightSides[i] = 0.5;
            }
        }

        protected void CalculatePressureDisbalance()
        {
            Array.Clear(_pressureDisbalance, 0, Graph.Arcs.Count);
            for (int i = 0; i < Graph.Arcs.Count; i++)
            {
                var arc = Graph.Arcs[i];
                int indexBegin = _allNodes.IndexOf(arc.BeginNode);
                int indexEnd = _allNodes.IndexOf(arc.EndNode);
                _pressureDisbalance[i] = (_p_nodes[indexBegin] - _p_nodes[indexEnd] - _pressureLooses[i])
                    / _derivatives[i];
            }
        }

        protected double CalculateFlowsInNodes()
        {
            Array.Clear(_q_nodes, 0, _q_nodes.Length);
            double disbalanceRes = 0;
            double maxDisbalance = 0;
            for (int i = 0; i < _allNodes.Count; i++)
            {
                _allNodes[i].Pcalc = Math.Sqrt(Math.Abs(_p_nodes[i])) * Math.Sign(_p_nodes[i]);
                _allNodes[i].Qloss = 0;
                foreach (var arc in _allNodes[i].ArcsOut)
                {
                    int index = _allArcs.IndexOf(arc);
                    _q_nodes[i] += _flows[index];
                }
                foreach (var arc in _allNodes[i].ArcsIn)
                {
                    int index = _allArcs.IndexOf(arc);
                    _q_nodes[i] -= _flows[index];
                    arc.FlowLoos = arc.CurrentModel().CalculateFlowLoss(arc.BeginNode.Pcalc, arc.EndNode.Pcalc, _flows[index]);

                    _allNodes[i].Qloss += arc.FlowLoos;
                }
                _allNodes[i].Qcalc = _q_nodes[i] - _allNodes[i].Qloss;
                if (_allNodes[i].SignPQ != Enums.NodeBoundarySign.Pressure)
                {
                    double d = Math.Abs(_q_nodes[i] - _allNodes[i].Q);
                    if (d > maxDisbalance && d > FlowAccuracy)
                    {
                        maxDisbalance = d;
                        disbalanceRes = d;
                    }
                }
            }

            foreach (var arc in Graph.Arcs)
            {
                arc.QEnd = arc.QBegin + arc.FlowLoos;
            }

            return disbalanceRes;
        }

        protected double CalculateDisbalance()
        {
            double result = 0;
            int arcId = 0;
            for (int i = 0; i < Graph.Arcs.Count; i++)
            {
                var arc = Graph.Arcs[i];
                int indexBegin = _allNodes.IndexOf(arc.BeginNode);
                int indexEnd = _allNodes.IndexOf(arc.EndNode);
                double loose = arc.CurrentModel()
                    .CalculatePressureLoss(arc.BeginNode.Pcalc, arc.EndNode.Pcalc, _flows[i]);
                double d = Math.Abs(_p_nodes[indexBegin] - _p_nodes[indexEnd] - loose - 0 * _pressureLooses[i]);
                if (d > result)
                {
                    result = d;
                    arcId = arc.Id;
                }
            }
            //Console.WriteLine(arcId);
            return result;
        }

        #endregion
    }
}
