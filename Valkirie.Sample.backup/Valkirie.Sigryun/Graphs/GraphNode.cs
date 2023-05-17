using System;
using System.Collections.Generic;
using Valkyrie.Sigryun.Enums;

namespace Valkyrie.Sigryun.Graphs
{
    /// <summary>
    /// Представляет узел расчетного графа
    /// </summary>
    public class GraphNode : GraphObject
    {
        #region Приватные переменные

        /// <summary>
        /// Список дуг на входе
        /// </summary>
        private List<GraphArc> _arcsIn;

        /// <summary>
        /// Список дуг на выходе
        /// </summary>
        private List<GraphArc> _arcsOut;

        /// <summary>
        /// Список эквивалентированных в узел моделей объектов
        /// </summary>
        private List<IComputationalModel> _ignorableModels;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает узел расчетного графа
        /// </summary>
        /// <param name="id">Идентификатор узла</param>
        /// <param name="parentGraph">Родительский граф</param>
        internal GraphNode(int id, Graph parentGraph)
            : base(id, parentGraph)
        {
            _arcsIn = new List<GraphArc>();
            _arcsOut = new List<GraphArc>();
            _ignorableModels = new List<IComputationalModel>();
            SignPQ = NodeBoundarySign.None;
        }

        #endregion

        #region Свойства

        #region Исходные параметры

        /// <summary>
        /// Давление
        /// </summary>
        public double P
        {
            get;
            set;
        }

        /// <summary>
        /// Расход
        /// </summary>
        public double Q
        {
            get;
            set;
        }

        /// <summary>
        /// Температура
        /// </summary>
        public double T
        {
            get;
            set;
        }

        /// <summary>
        /// Признак заданного давления или расхода
        /// </summary>
        public NodeBoundarySign SignPQ
        {
            get;
            set;
        }

        /// <summary>
        /// Дуги входящие из узлы
        /// </summary>
        public List<GraphArc> ArcsIn
        {
            get
            {
                return _arcsIn;
            }
        }

        /// <summary>
        /// Дуги исходящие из узла
        /// </summary>
        public List<GraphArc> ArcsOut
        {
            get
            {
                return _arcsOut;
            }
        }

        /// <summary>
        /// Получает список моделей объектов, эквивалентированных в узел
        /// </summary>
        public List<IComputationalModel> IgnorableModels
        {
            get
            {
                return _ignorableModels;
            }
        }

        #endregion

        #region Расчетные параметры

        /// <summary>
        /// Давление расчетное
        /// </summary>
        public double Pcalc
        {
            get;
            set;
        }

        /// <summary>
        /// Расход расчетный
        /// </summary>
        public double Qcalc
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает расчетные потери потока
        /// </summary>
        public double Qloss
        {
            get;
            set;
        }

        /// <summary>
        /// Температура расчетная
        /// </summary>
        public double Tcalc
        {
            get;
            set;
        }

        #endregion

        #region Заданные ограничения

        /// <summary>
        /// Получает или задает минимальное давление в узле
        /// </summary>
        public double Pmin
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает максимальное давление в узле
        /// </summary>
        public double Pmax
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает минимальный расход в узле
        /// </summary>
        public double Qmin
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает максимальный расход в узле
        /// </summary>
        public double Qmax
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Методы

        /// <summary>
        /// Функция создает копию объекта
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            throw new NotImplementedException();

            GraphNode result = new GraphNode(this.Id, null);
            result.P = this.P;
            result.Pcalc = this.Pcalc;
            result.Q = this.Q;
            result.Qcalc = this.Qcalc;
            result.SignPQ = this.SignPQ;
            result.T = this.T;
            result.Tcalc = this.Tcalc;

            return result;
        }

        #endregion
    }
}
