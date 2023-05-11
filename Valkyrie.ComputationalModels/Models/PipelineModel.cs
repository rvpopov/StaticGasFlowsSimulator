using System;
using Valkyrie.Sigryun;

namespace Valkyrie.ComputationalModels.Models
{
    public class PipelineModel
    {
        #region Перечисления

        public enum ZMethods
        {
            Constant,
            Normative2005
        }

        public enum LambdaMethods
        {
            Constant,
            Turbulent,
            Normative2005
        }

        #endregion

        #region Приватные переменные

        protected double _zConstant = 0.87;
        protected double _lambdaConstant = 0.012;

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает объект, реализующий модель трубопровода
        /// </summary>
        public PipelineModel()
        {
            FlowProps = new FlowProperties(0.687);
            TAvg = 275.15;
            ZMethod = ZMethods.Normative2005;
            LambdaMethod = LambdaMethods.Normative2005;
        }

        #endregion

        #region Свойства

        //TODO: надо что-то делать с этой батвой
        public FlowProperties FlowProps
        {
            get;
            set;
        }

        /// <summary>
        /// Гидравлическая эффективность
        /// </summary>
        public double HydraulicEffeciency
        {
            get;
            set;
        }

        /// <summary>
        /// Коэффикиент теплопередачи
        /// </summary>
        public double HeatTransfer
        {
            get;
            set;
        }

        /// <summary>
        /// Температура окружающей среды
        /// </summary>
        public double EnvironmentTemperature
        {
            get;
            set;
        }

        /// <summary>
        /// Минимальное давление в трубопроводе
        /// </summary>
        public double PMin
        {
            get;
            set;
        }

        /// <summary>
        /// Максимальное давление в трубопроводе
        /// </summary>
        public double PMax
        {
            get;
            set;
        }

        /// <summary>
        /// Внутренний диаметр
        /// </summary>
        public double Din
        {
            get;
            set;
        }

        /// <summary>
        /// Наружний диаметр
        /// </summary>
        public double Dout
        {
            get;
            set;
        }

        /// <summary>
        /// Длина трубопровода
        /// </summary>
        public double Length
        {
            get;
            set;
        }

        /// <summary>
        /// Эквивалентная шероховатость
        /// </summary>
        public double Roughness
        {
            get;
            set;
        }

        /// <summary>
        /// Серднее давление по трубопроводу
        /// </summary>
        public double PAvg
        {
            get;
            set;
        }

        /// <summary>
        /// Средняя температура по трубопроводу
        /// </summary>
        public double TAvg
        {
            get;
            set;
        }

        /// <summary>
        /// Коэффициент гидравлического сопротивления
        /// </summary>
        public double A
        {
            get;
            set;
        }

        /// <summary>
        /// Коэффициент запаса газа
        /// </summary>
        public double B
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает метод вычисления коэффициента сжимаемости газа
        /// </summary>
        public ZMethods ZMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает метод вычисления коэффициента сжимаемости газа
        /// </summary>
        public LambdaMethods LambdaMethod
        {
            get;
            set;
        }

        #region Параметры расчетные газового потока

        /// <summary>
        /// Получает расчетное значние давления газа в начале газопровода
        /// </summary>
        public double Pin
        {
            get;
            protected set;
        }

        /// <summary>
        /// Получает расчетное значние давления газа в конце газопровода
        /// </summary>
        public double Pout
        {
            get;
            protected set;
        }

        /// <summary>
        /// Получает расчетное значние расхода газа в начале газопровода
        /// </summary>
        public double Qin
        {
            get;
            protected set;
        }

        /// <summary>
        /// Получает расчетное значние расхода газа в конце газопровода
        /// </summary>
        public double Qout
        {
            get;
            protected set;
        }

        /// <summary>
        /// Получает расчетное значние температуры газа в начале газопровода
        /// </summary>
        public double Tin
        {
            get;
            protected set;
        }

        /// <summary>
        /// Получает расчетное значние температуры газа в конце газопровода
        /// </summary>
        public double Tout
        {
            get;
            protected set;
        }

        #endregion

        #endregion

        #region Методы

        public object Clone()
        {
            throw new NotImplementedException();
            //var res = new PipelineModel();
            //res.A = this.A;
            //res.Din = this.Din;
            //res.Dout = this.Dout;
            //res.EnvironmentTemperature = this.EnvironmentTemperature;
            //res.FlowProps = (FlowProperties)this.FlowProps.Clone();
            //res.HeatTransfer = this.HeatTransfer;
            //res.HydraulicEffeciency = this.HydraulicEffeciency;
            //res.IsRecalcPMinMax = this.IsRecalcPMinMax;
            //res.Length = this.Length;
            //res.PAvg = this.PAvg;
            //res.PMax = this.PMax;
            //res.PMin = this.PMin;
            //res.Roughness = this.Roughness;
            //res.TAvg = this.TAvg;
            //return res;
        }

        #endregion

        #region Приватные методы

        /// <summary>
        /// Функция вычисляет расход газа по трубопроводу
        /// </summary>
        /// <param name="pin">Давление в начале трубопровода</param>
        /// <param name="pout">Давление в конце трубопровода</param>
        /// <returns>Расход газа</returns>
        protected virtual double GetQ(double pin, double pout)
        {
            double sign = 1;
            if (pin < pout)
            {
                Swap(ref pin, ref pout);
                sign = -1;
            }
            //Вычислем среднее давление
            PAvg = GetPavg(pin, pout);
            //вычисляем свойства газового потока
            //Коэффициент сжимаемости газа
            double z = FlowProps.GetZ(PAvg, TAvg);
            //Относительная плотность газа
            double delta = FlowProps.GetDelta();
            //Задается начальный коэффициент гидравлического сопротивления
            double lm = 0.01;
            //Задается начальный расход
            double q = 3.32 * Math.Pow(10, -6) * Math.Pow(Din, 2.5) *
                    Math.Sqrt((Math.Pow(pin, 2) - Math.Pow(pout, 2)) / (lm * delta * TAvg * z * Length));
            //double q = 0;
            //for (int i = 0; i < 100; i++)
            //{
            //    //Расчитывается расход газа по формуле НТП МГ
            //    double qn = 3.32 * Math.Pow(10, -6) * Math.Pow(Din, 2.5) *
            //        Math.Sqrt((Math.Pow(pin, 2) - Math.Pow(pout, 2)) / (lm * delta * TAvg * z * Length));
            //    //lm = GetLambda(qn, PAvg, TAvg, Din, delta);
            //    //Проверяем условие окончания процедуры
            //    if (Math.Abs((q - qn) / qn) < 0.00000001)
            //    {
            //        q = qn;
            //        break;
            //    }
            //    //Пересчитываем коэффициент гидравлического сопротивления
            //    //lm = GetLambda(Math.Abs(qn), PAvg, TAvg, Din, delta);
            //    q = qn;
            //}
            if (double.IsNaN(q))
            {
            }
            return q * sign;
        }

        public double GetTout(double pin, double pout, double tin, double qin)
        {
            if (pin < pout || qin < 0)
            {
                Swap(ref pin, ref pout);
                qin = Math.Abs(qin);
            }
            if (pin < 0)
                pin = 1;
            if (pout < 0)
                pout = 1;
            PAvg = GetPavg(pin, pout);
            double z = FlowProps.GetZ(PAvg, TAvg);
            double delta = FlowProps.GetDelta();
            double cp = FlowProps.GetCp(PAvg, TAvg, delta);
            double alpha = 225.5 * HeatTransfer * Dout / (qin * delta * cp * Math.Pow(10, 6));
            double di = FlowProps.GetDi(PAvg, TAvg);

            //double di = (FlowProps.GetDi(pin, tin) + FlowProps.GetDi(pout, Tout)) / 2;
            //double di = 0;
            //int k = 1000;
            //double h = Length / k;
            //for (int i = 0; i < k; i++)
            //{
            //    double pin_c = Math.Sqrt(pin * pin - A * qin * qin * k * h / Length);
            //    double pout_c = Math.Sqrt(pin * pin - A * qin * qin * (k + 1) * h / Length);
            //    di += (FlowProps.GetDi(pin_c, TAvg) + FlowProps.GetDi(pout_c, TAvg)) * h / (2 * Length);
            //}
            //di = 0;
            double tout = EnvironmentTemperature + (tin - EnvironmentTemperature) * Math.Exp(-alpha * Length) -
                di * (Math.Pow(pin, 2) - Math.Pow(pout, 2)) * (1 - Math.Exp(-alpha * Length)) / (2 * alpha * Length * PAvg);

            TAvg = GetTavg(qin, Math.Abs(pin), Math.Abs(pout), tin, di, alpha);
            Tin = tin;
            Tout = tout;

            return tout;
        }

        protected void CalculateSpeeds(double pin, double pout, double qin)
        {
            double z = FlowProps.GetZ(PAvg, TAvg);
            //Parent.VBegin = 1.223 * 41.6666 * Math.Abs(qin) * TAvg * z / (Math.Abs(pin) * Din * Din / 100);
            //Parent.VEnd = 1.223 * 41.6666 * Math.Abs(qin) * TAvg * z / (Math.Abs(pout) * Din * Din / 100);
        }

        /// <summary>
        /// Функция вычисляет среднее давление
        /// </summary>
        /// <param name="pIn">Давление на входе</param>
        /// <param name="pOut">Давление на выходе</param>
        /// <returns>Среднее давление по трубопродоводу</returns>
        protected double GetPavg(double pIn, double pOut)
        {
            if (pIn < 0)
                pIn = 1;
            if (pOut < 0)
                pOut = 1;
            if (pIn < pOut)
                Swap(ref pIn, ref pOut);
            return 2 * (pIn + Math.Pow(pOut, 2) / (pIn + pOut)) / 3;
        }

        protected double GetTavg(double q, double pin, double pout, double tin, double di, double alpha)
        {
            double res = EnvironmentTemperature + (tin - EnvironmentTemperature) * (1 - Math.Exp(-alpha * Length)) / (alpha * Length) -
                di * (Math.Pow(pin, 2) - Math.Pow(pout, 2)) * (1 - (1 - Math.Exp(-alpha * Length)) / (alpha * Length)) / (2 * alpha * Length * PAvg);

            return res;
        }

        /// <summary>
        /// Функция возвращает коэффициент гидравлического сопротивления
        /// </summary>
        /// <param name="Q">Расход</param>
        /// <param name="P">Среднее давление</param>
        /// <param name="T">Средняя температура</param>
        /// <param name="Din">Внутренний диаметр</param>
        /// <param name="delta">Относительная плотность газа по воздуху</param>
        /// <returns>Значение коэффициента гидравлического сопротивления</returns>
        public double GetLambda(double Q, double P, double T, double Din, double delta)
        {
            //Вычисляем динамическую вязкость
            double mu = FlowProps.GetMu(P, T);
            //Число Рейнольдса
            double Re = 17.75 * Math.Pow(10, 3) * Q * delta / (Din * mu);
            double lambda_tr = 0.067 * Math.Pow((158 / Re + 2 * Roughness / Din), 0.2);

            //Коэффициент гидравлического сопротивления
            double lambda = lambda_tr / Math.Pow(HydraulicEffeciency, 2);
            return lambda;
        }

        /// <summary>
        /// Функция меняет местами значения
        /// </summary>
        /// <param name="a">Значение 1</param>
        /// <param name="b">Значение 2</param>
        protected void Swap(ref double a, ref double b)
        {
            double tmp = a;
            a = b;
            b = tmp;
        }


        #region Расчет параметров модели

        /// <summary>
        /// Расчет коэффициент в уравнении сохранения количества движения для i-ой секции
        /// </summary>
        /// <returns>Результирующий коэффициент</returns>
        public virtual double GetCoefficientA(double pin, double pout, double q, double tavg)
        {
            //Среднее давление
            double pavg = GetPavg(pin, pout);
            //Относительная плотность
            double delta = FlowProps.GetDelta();

            //Вычисление коэффициента гидравлического сопративления
            double lambda = _lambdaConstant;
            if (LambdaMethod == LambdaMethods.Turbulent)
                lambda = 0.067 * Math.Pow(2 * Roughness / Din, 0.2) / (HydraulicEffeciency * HydraulicEffeciency);
            if (LambdaMethod == LambdaMethods.Normative2005)
                lambda = GetLambda(Math.Abs(q), pavg, tavg, Din, FlowProps.GetDelta());

            //Вычисление коэффициента сжимаемости газа
            double z = _zConstant;
            if (ZMethod == ZMethods.Normative2005)
                z = FlowProps.GetZ(pavg, tavg);

            double res = 9.0417 * Math.Pow(10, 10) * lambda * z * tavg * delta * Length / Math.Pow(Din, 5);

            return res;
        }

        /// <summary>
        /// Расчет коэффициент в уравнении сохранения массы для i-ого узла
        /// </summary>
        /// <returns></returns>
        public virtual double GetCoefficientB(double pin, double pout, double tavg)
        {
            double pavg = GetPavg(pin, pout);

            //Вычисление коэффициента сжимаемости газа
            double z = _zConstant;
            if (ZMethod == ZMethods.Normative2005)
                z = FlowProps.GetZ(pavg, tavg);

            double B = 5.096 * z * tavg / (Math.Pow(Din, 2) * Length);
            return B;
        }

        #endregion

        #endregion

    }
}
