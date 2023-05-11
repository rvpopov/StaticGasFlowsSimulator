# Библиотека гидравлических расчетов газотранспортных систем Valkirie
Библиотека предназначена для проведения гидравлических расчетов стационарных режимов функционирования газотранспортных систем (ГТС).

В состав библиотеки входят модели следующих технологических объектов:
- Аппарат воздушного охлаждения газа (АВО);
- Перемычка газопровода;
- Пылеуловитель (ПУ);
- Газоперекачивающий агрегат (ГПА)
- Объект гидравлических потерь;
- Трубопровод;
- Кран;
- Вход/выход ГТС.

В основу расчета течения газа положен метод глобального градиента.

Проведение расчета осуществляется в три этапа.

## Этап 1. Формируются модели объектов ГТС
На этом этапе задаются параметры технологических объектов ГТС.
```
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
    FlowProps = new FlowProperties(0.687),
    HeatTransfer = 1.3,
    HydraulicEffeciency = 0.95,
    LambdaMethod = PipelineModel.LambdaMethods.Normative2005,
    Length = 120,
    PMax = SConverter.AtaToMPa(75),
    Roughness = 0.03,
    ZMethod = PipelineModel.ZMethods.Normative2005
};

var pipeline2 = new StaticGGAPipeline()
{
    Din = 1387,
    Dout = 1420,
    EnvironmentTemperature = SConverter.CToK(5),
    FlowProps = new FlowProperties(0.687),
    HeatTransfer = 1.3,
    HydraulicEffeciency = 0.95,
    LambdaMethod = PipelineModel.LambdaMethods.Normative2005,
    Length = 130,
    PMax = SConverter.AtaToMPa(75),
    Roughness = 0.03,
    ZMethod = PipelineModel.ZMethods.Normative2005
};

var pipeline3 = new StaticGGAPipeline()
{
    Din = 1387,
    Dout = 1420,
    EnvironmentTemperature = SConverter.CToK(5),
    FlowProps = new FlowProperties(0.687),
    HeatTransfer = 1.3,
    HydraulicEffeciency = 0.95,
    LambdaMethod = PipelineModel.LambdaMethods.Normative2005,
    Length = 130,
    PMax = SConverter.AtaToMPa(75),
    Roughness = 0.03,
    ZMethod = PipelineModel.ZMethods.Normative2005
};
```

## Этап 2. Формирование топологии расчетного графа
На данном этапе производится объединение технологических объектов в газотранспортную сеть. Объекты схемы (входы, выходы, ГПА, краны, трубы и т.д.) моделируются в виде узлов.
```
//            /---pipeline1---\
//           /                 \
// source---n1                 n2---pipeline3---n3---exit
//           \                 /
//            \---pipeline2---/
var graph = new HierarchicalGraph();
var sourceNode = graph.CreateComputationalNode(source);
var exitNode = graph.CreateComputationalNode(exit);
var pipelineNode1 = graph.CreateComputationalNode(pipeline1);
var pipelineNode2 = graph.CreateComputationalNode(pipeline2);
var pipelineNode3 = graph.CreateComputationalNode(pipeline3);

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
```
В примере ``n1, n2, n3`` - порты, через скоторые стыкуются технологические объекты ГТС. Есть условие, что в моделируемый узел может входить не большей одной дуги и сходить из него тоже не больше одной дуги

## Этап 3. Запуск расчета
На этом этапе инициализируется расчетная задача, на вход которой предоставляется топология ГТС с параметризированными технологическими объектами.
```
StaticRegimeTask calculator = new StaticRegimeTask();
calculator.Calculate(graph);
```

## Этап 4. Считывание результатов расчета
Пример получения результатов расчета представлен ниже.
```
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
```

## Заключение
В архитектуру модуля заложены концепции многоуровневого иерархического представляния схем ГТС. По мере возможности они будут развиваться.
