using System;
using System.Linq;
using Valkyrie.HierarchicalModel.Graphs;
using Valkyrie.HierarchicalModel.Helpers;
using Valkyrie.Sigryun.Calculators.StaticGGA;
using Valkyrie.Sigryun.Calculators.TermalCalculator;
using Valkyrie.Sigryun.Graphs;

namespace Valkyrie.ComputationalTasks.StaticRegime
{
    /// <summary>
    /// Представляет задачу расчета стационарного тепло-гидравлического режима работы ГТС
    /// </summary>
    public class StaticRegimeTask
    {
        #region Приватные переменные

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает задачу расчета стационарного тепло-гидравлического режима работы ГТС
        /// </summary>
        public StaticRegimeTask()
        {
            MaximunExternalIterations = 100;
            MaximumHydraulicIterations = 100;
            PressureAccuracy = 0.0001;
            FlowAccuracy = 0.000001;
            TempretureAccuracy = 0.0001;
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получает или задает максимальное количество внешних итераций
        /// </summary>
        public int MaximunExternalIterations
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает максимальное количество итераций по гидравлике
        /// </summary>
        public int MaximumHydraulicIterations
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает точность по давлению, МПа
        /// </summary>
        public double PressureAccuracy
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает точность по расходу, млн.м3/сут
        /// </summary>
        public double FlowAccuracy
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает точность по температуре, K
        /// </summary>
        public double TempretureAccuracy
        {
            get;
            set;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Вызывает процедуру расчета
        /// </summary>
        /// <param name="graph">Иерархический граф ГТС</param>
        public virtual void Calculate(HierarchicalGraph graph)
        {
            //Исходный иерархический граф граф необходимо преобразовать в расчетный
            var computationalGraph = CreateComputationalGraph(graph);
            Calculate(computationalGraph);
            graph.UpdateHierarchicalComputationalResults(computationalGraph);
        }

        /// <summary>
        /// Производит теплогидравлический расчет модели ГТС
        /// </summary>
        /// <param name="computationalGraph">Расчетный граф ГТС</param>
        public virtual void Calculate(Graph computationalGraph)
        {
            if (computationalGraph.Nodes.Count > 0)
            {
                StaticGGACalculator hydraulicCalculator = new StaticGGACalculator();
                hydraulicCalculator.MaximumIterations = MaximumHydraulicIterations;
                hydraulicCalculator.PressureAccuracy = PressureAccuracy;
                hydraulicCalculator.FlowAccuracy = FlowAccuracy;
                hydraulicCalculator.GenerateStartConditions(computationalGraph);

                TermalCalculator termalCalculator = new TermalCalculator();

                hydraulicCalculator.Calculate(computationalGraph);
                termalCalculator.Calculate(computationalGraph);

                var pressures0 = computationalGraph.Nodes.Select(o => o.Pcalc).ToArray();
                var flowsStart0 = computationalGraph.Arcs.Select(o => o.QBegin).ToArray();
                var flowsFinish0 = computationalGraph.Arcs.Select(o => o.QEnd).ToArray();
                var temperatures0 = computationalGraph.Nodes.Select(o => o.Tcalc).ToArray();

                bool isNormalResult = false;
                for (int i = 0; i < MaximunExternalIterations; i++)
                {
                    hydraulicCalculator.Calculate(computationalGraph);
                    termalCalculator.Calculate(computationalGraph);

                    var pressures1 = computationalGraph.Nodes.Select(o => o.Pcalc).ToArray();
                    var flowsStart1 = computationalGraph.Arcs.Select(o => o.QBegin).ToArray();
                    var flowsFinish1 = computationalGraph.Arcs.Select(o => o.QEnd).ToArray();
                    var temperatures1 = computationalGraph.Nodes.Select(o => o.Tcalc).ToArray();

                    var pressureNorm = CalculateNormMax(pressures0, pressures1);
                    var flowStartNorm = CalculateNormMax(flowsStart0, flowsStart1);
                    var flowFinishNorm = CalculateNormMax(flowsFinish0, flowsFinish1);
                    var temperatureNorm = CalculateNormMax(temperatures0, temperatures1);
                    if (pressureNorm < hydraulicCalculator.PressureAccuracy
                        && flowStartNorm < hydraulicCalculator.FlowAccuracy
                        && flowFinishNorm < hydraulicCalculator.FlowAccuracy
                        && temperatureNorm < TempretureAccuracy)
                    {
                        //Console.WriteLine("Внешних итераций " + (i + 1));
                        isNormalResult = true;
                        break;
                    }
                    pressures0 = pressures1;
                    flowsStart0 = flowsStart1;
                    flowsFinish0 = flowsFinish1;
                    temperatures0 = temperatures1;
                }
                if (!isNormalResult)
                    Console.WriteLine("Плохой результат");
            }
        }

        #endregion

        #region Приватные методы

        protected double CalculateNormMax(double[] vector0, double[] vector1)
        {
            double maxNorm = 0;
            for (int i = 0; i < vector0.Length; i++)
            {
                double diff = Math.Abs(vector0[i] - vector1[i]);
                if (diff > maxNorm)
                    maxNorm = diff;
            }
            return maxNorm;
        }

        protected Graph CreateComputationalGraph(HierarchicalGraph graph)
        {
            HierarchicalModelConverter converter = new HierarchicalModelConverter();
            var computationalGraph = converter.CreateComputationalGraph(graph);
            computationalGraph.EquivalentIngnorableArcs();
            computationalGraph.RemoveDisabledArcs();
            converter.CreateFlowSources(computationalGraph);
            computationalGraph.RemoveDanglingNodes();
            return computationalGraph;
        }

        #endregion
    }
}
