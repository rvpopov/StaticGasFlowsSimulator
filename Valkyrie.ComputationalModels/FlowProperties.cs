using System;

namespace Valkyrie.ComputationalModels
{
    public class FlowProperties : ICloneable
    {
        #region Перечисления

        public enum ZMethods
        {
            Constant,
            NTPMG2006,
            GOST30139_3_2015
        }
        #endregion
        public FlowProperties(double ro, double xa, double xy)
        {
            Ro = ro;
            Xa = xa;
            Xy = xy;
            CalculatePsevdoCriticalValues();
        }

        #region Свойства

        public double Ppk
        {
            get;
            private set;
        }

        public double Tpk
        {
            get;
            private set;
        }
        /// <summary>
        /// Плотность газа
        /// </summary>
        public double Ro
        {
            get;
            set;
        }
        /// <summary>
        /// Доля Азота
        /// </summary>
        public double Xa
        {
            get;
            private set;
        }
        /// <summary>
        /// Доля Углерода
        /// </summary>
        public double Xy
        {
            get;
            private set;
        }

        /// <summary>
        /// Давление при стандратных условиях
        /// </summary>
        public double PStandart
        {
            get
            {
                return 0.101325;
            }
        }

        /// <summary>
        /// Температура при стандартных условиях
        /// </summary>
        public double TStandart
        {
            get
            {
                return 293.15;
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Фунция вычисляет коэффициент сжимаемости газа
        /// </summary>
        /// <param name="P">Давление газа</param>
        /// <param name="T">Температура</param>
        /// <returns>Коэффициент сжимаемости</returns>
        public double GetZ(double P, double T, ZMethods zMethods)
        {
            switch(zMethods)
            {
                    case ZMethods.Constant:
                        return 0.87;
                    case ZMethods.NTPMG2006:
                        return GetZNTPMG2006(P, T);
                    case ZMethods.GOST30139_3_2015:
                        return GetZGOST30139_3_2015(P, T);
            };
            return 0.87; //Константа, если неверный параметр
        }
        private double GetZNTPMG2006(double P, double T)
        {
            double Ppr = P / Ppk;
            double Tpr = T / Tpk;
            double A1 = -0.39 + 2.03 / Tpr - 3.16 / Math.Pow(Tpr, 2) + 1.09 / Math.Pow(Tpr, 3);
            double A2 = 0.0423 - 0.1812 / Tpr + 0.2124 / Math.Pow(Tpr, 2);
            double Z = 1 + A1 * Ppr + A2 * Math.Pow(Ppr, 2);
            return Z;
        }

        private double GetZGOST30139_3_2015(double P, double T)
        {

            throw new NotImplementedException();
        }

            /// <summary>
            /// Расчёт коэф. сжимаемости при С.У. по ГОСТ 30319ю2-2015 по неполному комп. составу
            /// </summary>
            /// <returns></returns>
            public double GetZCNotFull()
        {
            double _tmp = (0.0741 * Ro - 0.006 - 0.0063 * Xa - 0.0575 * Xy);
            return 1 - _tmp * _tmp;
        }

            /// <summary>
            /// Функция возвращает относительную плотность по воздуху
            /// </summary>
            /// <param name="Z">Коэффициент сжимаемости газа</param>
            /// <returns></returns>
            public double GetDelta()
        {
            double Rov = 1.20445; //Плотность воздуха
            double delta = Ro / Rov;
            return delta;
        }

        /// <summary>
        /// Функция вычисляет универсальную газовую постоянную
        /// </summary>
        /// <returns>Значение универсальной газовой постоянно</returns>
        public double GetR(double delta)
        {
            return 101325 / (Ro * 293.15 * GetZCNotFull());
            double rr = 0.28689 / delta;
            return rr;

            //
            //return R;
        }

        /// <summary>
        /// Функция вычисляет универсальную газовую постоянную
        /// </summary>
        /// <returns>Значение универсальной газовой постоянно</returns>
        public double GetR()
        {
            double delta = GetDelta();
            double zs = 1 - Math.Pow(0.0741 * Ro - 0.006, 2);
            double R = 101.325 / (Ro * 293.15 * zs);
            double rr = 0.287 / delta;
            return R;
        }

        /// <summary>
        /// Функция возвращает динамеческую вязкость природного газа
        /// </summary>
        /// <param name="P"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public double GetMu(double P, double T)
        {
            double Ppr = P / Ppk;
            double Tpr = T / Tpk;

            //Динамическая вязкость природного газа
            double mu0 = (1.81 + 5.95 * Tpr) * Math.Pow(10, -6);
            double B1 = -0.67 + 2.36 / Tpr - 1.93 / Math.Pow(Tpr, 2);
            double B2 = 0.8 - 2.89 / Tpr + 2.65 / Math.Pow(Tpr, 2);
            double B3 = -0.1 + 0.354 / Tpr - 0.314 / Math.Pow(Tpr, 2);
            double mu = mu0 * (1 + B1 * Ppr + B2 * Math.Pow(Ppr, 2) + B3 * Math.Pow(Ppr, 3));
            return mu;
        }
        /// <summary>
        /// Функция расчёта изобарной теплоёмкости газа по НТП МГ 2006
        /// </summary>
        /// <param name="P">Давление, Па</param>
        /// <param name="T">Температура, К</param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public double GetCp(double P, double T, double delta)
        {
            double Ppr = P / Ppk;
            double Tpr = T / Tpk;
            double R = GetR(delta);


            double E0 = 4.437
                - 1.015 * Tpr
                + 0.591 * Tpr * Tpr;
            double E1 = 3.29
                - 11.37 / Tpr
                + 10.9 / (Tpr * Tpr);
            double E2 = 3.23
                - 16.27 / Tpr
                + 25.48 / (Tpr * Tpr)
                - 11.81 / (Tpr * Tpr * Tpr);
            double E3 = -0.214
                + 0.908 / Tpr
                - 0.967 / (Tpr * Tpr);

            return R * (E0 + E1 * Ppr + E2 * Ppr * Ppr + E3 * Ppr * Ppr * Ppr);
        }
        /// <summary>
        /// Функция определения коэф. Джоуля - Томсона НТП МГ -2006
        /// </summary>
        /// <param name="P">Давление, Па</param>
        /// <param name="T">Температура, К</param>
        /// <returns></returns>
        public double GetDi(double P, double T)
        {
            double Ppr = P / Ppk;
            double Tpr = T / Tpk;

            double H0 = 24.96
                - 20.3 * Tpr
                + 4.57 * Tpr * Tpr;
            double H1 = 5.66
                - 19.92 / Tpr
                + 16.89 / (Tpr * Tpr);
            double H2 = -4.11
                + 14.68 / Tpr
                - 13.39 / (Tpr * Tpr);
            double H3 = 0.568
                - 2.0 / Tpr
                + 1.79 / (Tpr * Tpr);
            double res = H0 + H1 * Ppr + H2 * Ppr * Ppr + H3 * Ppr * Ppr * Ppr;
            return res;
        }

        public double GetLowHeatingValue()
        {
            double xa = 0;
            double xy = 0;
            return 85453 * (0.52190 * Ro + 0.04242 - 0.65197 * xa - xy);
        }

        /// <summary>
        /// Копирует объект
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            FlowProperties res = new FlowProperties(Ro,Xa,Xy);
            return res;
        }

        #endregion

        #region Приватные методы

        /// <summary>
        /// Функция вычисляет псевдокритические параметры газа по формуле ГОСТ 30319.2-2015 для неполного компонентного состава газа
        /// </summary>
        private void CalculatePsevdoCriticalValues()
        {
            //double xy = 0.0;
            //double xa = 0.0;

            Ppk = 2.9585 * (1.608 - 0.05994 * Ro + Xy - 0.392 * Xa);
            Tpk = 88.25 * (0.9915 + 1.759 * Ro - Xy - 1.681 * Xa);
        }

        #endregion
    }
}
