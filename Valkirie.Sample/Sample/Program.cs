using System;
using Valkyrie.ComputationalModels;
using Valkyrie.ComputationalModels.CalculatorRealisations.StaticGGACalculator;
using Valkyrie.ComputationalModels.Models;
using Valkyrie.ComputationalTasks.StaticRegime;
using Valkyrie.HierarchicalModel.Graphs;
using Valkyrie.HierarchicalModel.Models;
using Valkyrie.Sigryun;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var graph = new HierarchicalGraph();

            //Создает модели объектов схемы
            var source = new EnterModel()
            {
                BoundarySign = Valkyrie.Sigryun.Enums.NodeBoundarySign.Pressure,
                Pressure = SConverter.AtaToMPa(52),
                Temperature = SConverter.CToK(18),
            };

            var exit = new ExitModel()
            {
                BoundarySign = Valkyrie.Sigryun.Enums.NodeBoundarySign.Pressure,
                Pressure = SConverter.AtaToMPa(42),
            };

            var pipeline1 = new StaticGGAPipeline()
            {
                Din = 1387,
                Dout = 1420,
                EnvironmentTemperature = SConverter.CToK(5),
                FlowProps = new FlowProperties(0.687,0,0),
                HeatTransfer = 1.3,
                HydraulicEffeciency = 0.95,
                LambdaMethod = PipelineModel.LambdaMethods.NTPMG2006,
                Length = 120,
                PMax = SConverter.AtaToMPa(75),
                Roughness = 0.03,
                ZMethod = FlowProperties.ZMethods.NTPMG2006
            };

            var pipeline2 = new StaticGGAPipeline()
            {
                Din = 1387,
                Dout = 1420,
                EnvironmentTemperature = SConverter.CToK(5),
                FlowProps = new FlowProperties(0.687, 0, 0),
                HeatTransfer = 1.3,
                HydraulicEffeciency = 0.95,
                LambdaMethod = PipelineModel.LambdaMethods.NTPMG2006,
                Length = 130,
                PMax = SConverter.AtaToMPa(75),
                Roughness = 0.03,
                ZMethod = FlowProperties.ZMethods.NTPMG2006
            };

            var pipeline3 = new StaticGGAPipeline()
            {
                Din = 1387,
                Dout = 1420,
                EnvironmentTemperature = SConverter.CToK(5),
                FlowProps = new FlowProperties(0.687, 0, 0),
                HeatTransfer = 1.3,
                HydraulicEffeciency = 0.95,
                LambdaMethod = PipelineModel.LambdaMethods.NTPMG2006,
                Length = 130,
                PMax = SConverter.AtaToMPa(75),
                Roughness = 0.03,
                ZMethod = FlowProperties.ZMethods.NTPMG2006
            };

            //Формируем граф:
            //            /---pipeline1---\
            //           /                 \
            // source---n1                 n2---pipeline3---n3---exit
            //           \                 /
            //            \---pipeline2---/

            //При разработке концепции расчетного модуля учитывался опыт "многоножек" в ИДТ
            //Концепция позволяет делать иерархически схемы с произвольной топологией на них
            //Объекты схемы (входы, выходы, ГПА, краны, трубы) моделируются узлами
            var sourceNode = graph.CreateComputationalNode(source);
            var exitNode = graph.CreateComputationalNode(exit);
            var pipelineNode1 = graph.CreateComputationalNode(pipeline1);
            var pipelineNode2 = graph.CreateComputationalNode(pipeline2);
            var pipelineNode3 = graph.CreateComputationalNode(pipeline3);

            //Это порты, через скоторые стыкуются дуги (см. рисунок)
            //Есть условие, что в моделируемый узел может входить не большей одной дуги и сходить из него тоже не больше одной дуги
            var n1 = graph.CreateNode();
            var n2 = graph.CreateNode();
            var n3 = graph.CreateNode();

            //Соединяем узлы моделируемых объектов с портами
            graph.CreateArc(sourceNode, n1);

            graph.CreateArc(n1, pipelineNode1);
            graph.CreateArc(pipelineNode1, n2);

            graph.CreateArc(n1, pipelineNode2);
            graph.CreateArc(pipelineNode2, n2);

            graph.CreateArc(n2, pipelineNode3);
            graph.CreateArc(pipelineNode3, n3);

            graph.CreateArc(n3, exitNode);

            //Создаем калькулятор и запускаем расчет
            StaticRegimeTask calculator = new StaticRegimeTask();
            calculator.Calculate(graph);

            //Выводим результат
            void printPipelineResult(StaticGGAPipeline p)
            {
                Console.WriteLine($"P start: {p.PressureStart:0.000}");
                Console.WriteLine($"P end:   {p.PressureFinish:0.000}");
                Console.WriteLine($"P avg:   {p.PAvg:0.000}");
                Console.WriteLine($"Q:       {p.FlowStart:0.000}");
                Console.WriteLine($"T start: {p.Tin:0.000}");
                Console.WriteLine($"T end:   {p.Tout:0.000}");
                Console.WriteLine($"T avg:   {p.TAvg:0.000}");
                Console.WriteLine("-------------------------------------");
                Console.WriteLine();
            }

            void printNodeResult(FlowSourceBase n)
            {
                Console.WriteLine($"P: {n.PressureCalc:0.000}");
                Console.WriteLine($"Q: {n.FlowCalc:0.000}");
                Console.WriteLine($"T: {n.TemperatureCalc:0.000}");
                Console.WriteLine("-------------------------------------");
                Console.WriteLine();
            }

            Console.WriteLine("Source result:");
            printNodeResult(source);

            Console.WriteLine("Pipeline1 result:");
            printPipelineResult(pipeline1);

            Console.WriteLine("Pipeline2 result:");
            printPipelineResult(pipeline2);

            Console.WriteLine("Pipeline3 result:");
            printPipelineResult(pipeline3);

            Console.WriteLine("Exit result:");
            printNodeResult(exit);

            Console.ReadKey();
        }
    }
}
