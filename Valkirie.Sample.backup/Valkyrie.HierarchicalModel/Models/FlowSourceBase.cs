using System;
using Valkyrie.Sigryun;
using Valkyrie.Sigryun.Interfaces;

namespace Valkyrie.HierarchicalModel.Models
{
    /// <summary>
    /// Представляет базовый класс поставщика/потребителя 
    /// </summary>
    public abstract class FlowSourceBase : IComputationalModel, IHydraulicNodeFinalize
    {
        /// <summary>
        /// Получает или задает тег объекта
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Признак того, что дуга должна быть удалена из графа
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Получает признак того, что дуга должна быть эквивалентирована в узел
        /// </summary>
        public bool IsIgnorable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Вызывает копирование объекта
        /// </summary>
        /// <returns></returns>
        public IComputationalModel Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получает или задает расчетный расхода газа
        /// </summary>
        public double FlowCalc
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает расчетное давление газа
        /// </summary>
        public double PressureCalc
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает расчетную температуру газа
        /// </summary>
        public double TemperatureCalc
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает расчетные свойства газового потока
        /// </summary>
        public IFlowProperties FlowPropertiesCalc
        {
            get;
            set;
        }

        #region Методы

        /// <summary>
        /// <summary>
        /// Производит обновление параметров модели по результатам расчета
        /// </summary>
        /// <param name="p">Давление в узле</param>
        /// <param name="t">Температура в узле</param>
        /// <param name="qin">Расход поступающий в узел</param>
        /// <param name="qout">Расход исходящий из узла</param>
        public abstract void FinalizeHydraulic(double p, double t, double qin, double qout);

        #endregion
    }
}
