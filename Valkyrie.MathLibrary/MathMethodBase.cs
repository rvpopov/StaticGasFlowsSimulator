using System;
using System.Collections.Generic;
using System.Linq;
using Valkyrie.MathLibrary.Events;

namespace Valkyrie.MathLibrary
{
    public abstract class MathMethodBase
    {
        #region Приватные переменные

        /// <summary>
        /// Список нижних значений переменных
        /// </summary>
        protected List<double> _lvals;
        /// <summary>
        /// Список верхних значений переменных
        /// </summary>
        protected List<double> _uvals;

        /// <summary>
        /// Словарь переводит название переменной в индект переменной в массиве
        /// </summary>
        protected Dictionary<string, int> _namesToIndex;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор абстрактного класса расчетного метода
        /// </summary>
        public MathMethodBase(FunctionalCallback obj)
        {
            _lvals = new List<double>();
            _uvals = new List<double>();
            _namesToIndex = new Dictionary<string, int>();
            Objective = obj;
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Список имен переменных
        /// </summary>
        public string[] VariableNames
        {
            get
            {
                return _namesToIndex.Keys.ToArray();
            }
        }

        /// <summary>
        /// Нижние границы значений переменных
        /// </summary>
        public double[] LowerValues
        {
            get
            {
                return _lvals.ToArray();
            }
        }

        /// <summary>
        /// Верхние границы значений переменных
        /// </summary>
        public double[] UpperValues
        {
            get
            {
                return _uvals.ToArray();
            }
        }

        /// <summary>
        /// Функция обратного вызова
        /// </summary>
        public FunctionalCallback Objective
        {
            get;
            private set;
        }

        /// <summary>
        /// Максимальное количество итераций
        /// </summary>
        public int MaxIterations
        {
            get;
            set;
        }

        #endregion

        #region Методы

        public int GetVariableIndex(string name)
        {
            if (_namesToIndex.ContainsKey(name))
                return _namesToIndex[name];
            else
                return -1;
        }

        #endregion

        #region Приватные переменные

        /// <summary>
        /// Вызывает обноваления статуса расчета
        /// </summary>
        /// <param name="objVal">Значение целевой функции</param>
        protected void OnIterationUpdate(double objVal, int iter = -1)
        {
            if (IterationUpdate != null)
            {
                IterationUpdate(this, new IterationEventArgs() { ObjVal = objVal, Iter = iter });
            }
        }

        #endregion

        #region Делегаты

        public delegate double FunctionalCallback(double[] x);

        #endregion

        #region События

        public event EventHandler<IterationEventArgs> IterationUpdate;

        #endregion

    }
}
