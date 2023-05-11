using Valkyrie.Sigryun;
using Valkyrie.Sigryun.Enums;

namespace Valkyrie.HierarchicalModel.Models
{
    public class EnterModel : VirtualEnterModel
    {
        /// <summary>
        /// Задает тип граничного условия
        /// </summary>
        public NodeBoundarySign BoundarySign
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает расхода газа
        /// </summary>
        public double Flow
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает давление газа
        /// </summary>
        public double Pressure
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает температуру газа
        /// </summary>
        public double Temperature
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает свойства газового потока
        /// </summary>
        public IFlowProperties FlowProperties
        {
            get;
            set;
        }

        /// <summary>
        /// <summary>
        /// Производит обновление параметров модели по результатам расчета
        /// </summary>
        /// <param name="p">Давление в узле</param>
        /// <param name="t">Температура в узле</param>
        /// <param name="qin">Расход поступающий в узел</param>
        /// <param name="qout">Расход исходящий из узла</param>
        public override void FinalizeHydraulic(double p, double t, double qin, double qout)
        {
            PressureCalc = p;
            TemperatureCalc = t;
            if (BoundarySign == NodeBoundarySign.Pressure)
            {
                double qdiff = qin - qout;
                FlowCalc = -qdiff;
            }
            else
                FlowCalc = Flow;
        }
    }
}
