using System;
using System.Collections.Generic;
using Valkyrie.ComputationalModels.Enums;
using Valkyrie.MathLibrary;

namespace Valkyrie.ComputationalModels.Models
{
    /// <summary>
    /// Класс реализует модель ГПА
    /// </summary>
    public class GasCompressorUnitModel
    {
        #region Приватные переменные

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает объект модели ГПА
        /// </summary>
        public GasCompressorUnitModel()
        {
            KPDKoefficients = new List<double>();
            PowerKoefficients = new List<double>();
            FlowProps = new FlowProperties(0.687, 0, 0);

            CoefficientAdaptationEfficiency = 0.95;
            CoefficientAdaptationPower = 0.95;
            TechnicalConditionCoefficient = 1.05;

            PenaltyCoeffPowerMax = 10e5;
            PenaltyCoeffQmax = 10e5;
            PenaltyCoeffQmin = 10e9;
            PenaltyCoeffRevolves = 10e6;

            //По-умолчанию НТП МГ 2006
            ZMethod = FlowProperties.ZMethods.NTPMG2006; 
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получает или задает метод вычисления коэффициента сжимаемости газа
        /// </summary>
        public FlowProperties.ZMethods ZMethod
        {
            get;
            set;
        }

        #region Номинальные параметры

        /// <summary>
        /// Минимальный приведенный расход
        /// </summary>
        public double QReducedMin
        {
            get;
            set;
        }

        /// <summary>
        /// Максимальный приведенный расход
        /// </summary>
        public double QReducedMax
        {
            get;
            set;
        }

        /// <summary>
        /// Минимальные относительные обороты
        /// </summary>
        public double RevsRelativeMin
        {
            get;
            set;
        }

        /// <summary>
        /// Максимальные относительные обороты
        /// </summary>
        public double RevsRelativeMax
        {
            get;
            set;
        }

        /// <summary>
        /// Номинальные оборты
        /// </summary>
        public double NominalRevs
        {
            get;
            set;
        }

        /// <summary>
        /// Номинальная мощность
        /// </summary>
        public double NominalPower
        {
            get;
            set;
        }

        /// <summary>
        /// КПД ЦБН
        /// </summary>
        public double KPDCBN
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает КПД газотурбинной установки
        /// </summary>
        public double GasTurbineEfficiency
        {
            get;
            set;
        }

        #endregion

        #region Параметры внешней среды

        public double Tair
        {
            get;
            set;
        }

        public double Pair
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Коэффициент влияния окружающей среды
        /// </summary>
        public double KEnvironment
        {
            get;
            set;
        }

        /// <summary>
        /// Номинальное атмосферное давление
        /// </summary>
        public double PAirNom
        {
            get;
            set;
        }

        /// <summary>
        /// Номинальная температура окружающей среды
        /// </summary>
        public double TAirNom
        {
            get;
            set;
        }

        /// <summary>
        /// Коэффициенты характеристики КПД
        /// </summary>
        public List<double> KPDKoefficients
        {
            get;
            private set;
        }

        /// <summary>
        /// Коэффициенты мощностной характеристики
        /// </summary>
        public List<double> PowerKoefficients
        {
            get;
            private set;
        }

        /// <summary>
        /// Обороты
        /// </summary>
        public double N
        {
            get;
            set;
        }

        //TODO: надо что-то делать с этой батой
        public FlowProperties FlowProps
        {
            get;
            private set;
        }

        #region Опретивные параметры

        public double Qin
        {
            get;
            set;
        }

        public double Pin
        {
            get;
            set;
        }

        public double Tin
        {
            get;
            set;
        }

        #endregion

        #region Параметры идентификации модели

        /// <summary>
        /// Получает или задает коэффициент адаптации характеристики КПД
        /// </summary>
        public double CoefficientAdaptationEfficiency
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает коэффициент адаптации характеристики мощности
        /// </summary>
        public double CoefficientAdaptationPower
        {
            get;
            set;
        }

        /// <summary>
        /// Получает или задает коэффициент технического состояния
        /// </summary>
        public double TechnicalConditionCoefficient
        {
            get;
            set;
        }

        #endregion

        #region Расчетные параметры

        /// <summary>
        /// Коэффициент загрузки
        /// </summary>
        public double Kload
        {
            get;
            set;
        }

        /// <summary>
        /// Коэффициент удаленности от зоны помпажа
        /// </summary>
        public double Ksurge
        {
            get;
            set;
        }

        /// <summary>
        /// Потребяемая мощность, МВт
        /// </summary>
        public double PowerConsumption
        {
            get;
            set;
        }

        /// <summary>
        /// Расчетное КПД
        /// </summary>
        public double EfficiencyCalc
        {
            get;
            set;
        }

        /// <summary>
        /// Получает расчетный расход топливного газа, млн.м3/сут
        /// </summary>
        public double FuelFlow
        {
            get;
            protected set;
        }

        #endregion

        #region Штрафы

        /// <summary>
        /// Текущий расчетный штраф за нарушение технолоческих ограничений
        /// </summary>
        public double CurrentPenalty
        {
            get;
            set;
        }

        public double PenaltyCoeffQmax
        {
            get;
            set;
        }

        public double PenaltyCoeffQmin
        {
            get;
            set;
        }

        public double PenaltyCoeffPowerMax
        {
            get;
            set;
        }

        public double PenaltyCoeffRevolves
        {
            get;
            set;
        }



        #endregion

        #endregion

        #region Методы

        public GasCompressorUnitResultEnum Calculate(double Pin, double Qin, double Tin, out double pout, out double tout, out double qfuel)
        {
            CurrentPenalty = 0;

            GasCompressorUnitResultEnum result = GasCompressorUnitResultEnum.NORMAL;
            //Приводим к нужным величинам оперативные параметры
            double n_otn = N / NominalRevs; //Относительные обороты

            //Расчитываем приведенный расход
            double z = FlowProps.GetZ(Pin, Tin, ZMethod);
            double zs = FlowProps.GetZ(FlowProps.PStandart, FlowProps.TStandart, ZMethod);
            double ros = FlowProps.Ro;//GetStandartRo(M, zs);
            double ro = ros * FlowProps.TStandart * Pin / (Tin * FlowProps.PStandart * z);
            double q_pri = 694.444 * Qin * ros / (ro * n_otn);

            if (q_pri > QReducedMax)
            {
                result = GasCompressorUnitResultEnum.QMAX;
                CurrentPenalty += PenaltyCoeffQmax * (q_pri - QReducedMax);
            }

            //Проверка коэффициента загрузки
            Ksurge = q_pri / QReducedMin;
            if (Ksurge < 1.1)
            {
                result = GasCompressorUnitResultEnum.KUD;
                CurrentPenalty += PenaltyCoeffQmin * (QReducedMin - q_pri);
            }

            //Расчет политропического КПД по характеристике нагнетателя
            double kpd = GetKPD(q_pri, CoefficientAdaptationEfficiency);
            EfficiencyCalc = kpd;

            //Расчет внутренней мощности нагнетателя
            double Ni = Math.Pow(n_otn, 3) * ro * GetPower(q_pri);
            PowerConsumption = Ni;
            //Расчет потребляемой мощности ГПА
            //Коэффициент, учитывающий влияение противобледенительной системы
            double K_ob = 1;
            //Коэффициент, учитывающий влияение системы утилизации выхлопных газов
            double K_y = 1;

            double Npotr = Ni / (KPDCBN * CoefficientAdaptationPower);

            //Расчет располагаемой мощности
            double Nrasp = NominalPower * CoefficientAdaptationPower * K_ob * K_y *
                (1 - KEnvironment * (Tair - TAirNom + 5) / (Tair + 5)) * Pair / PAirNom; //Внесена поправка на температуру 5 град.

            if (Nrasp / NominalPower > 1.15)
            {
                result = GasCompressorUnitResultEnum.POWER;
                //CurrentPenalty += PenaltyCoeffPowerMax * (Nrasp - 1.15 * NominalPower);
            }

            //Расчет резерва мощности и коэффициента загрузки
            double Nrez = Nrasp - Npotr;
            Kload = Npotr / Nrasp;
            if (Kload > 0.95)
            {
                result = GasCompressorUnitResultEnum.KLOAD;
                CurrentPenalty += 10e3 * PenaltyCoeffPowerMax * (Kload - 0.95);
            }
            //Расчет степени сжатия!
            double Ka = GetKa(Pin, Tin, ros);
            double R = FlowProps.GetR() * 1000;
            double Power = GetPower(q_pri) * 1000;
            double m = GetKPD(q_pri, CoefficientAdaptationEfficiency) * Ka / (GetKPD(q_pri, CoefficientAdaptationEfficiency) * Ka - Ka + 1);
            double A = Math.Pow(n_otn, 2) * Power * 60 * GetKPD(q_pri, CoefficientAdaptationEfficiency) / q_pri;
            double eps = Math.Pow((1 + (m - 1) * A / (z * Tin * R * m)), (m / (m - 1)));

            //Расчет топливного газа
            double qfuelNom = 3.6 * NominalPower / (GasTurbineEfficiency * 34500);
            double calorificHeatCoefficient = 34500 / FlowProps.GetLowHeatingValue();
            double environmentCoefficient = 0.75 * Npotr / NominalPower + 0.25 * Math.Sqrt(Tair / 288) * Pair / PAirNom;
            qfuel = qfuelNom * calorificHeatCoefficient * environmentCoefficient * TechnicalConditionCoefficient * 24 / 1000;
            FuelFlow = qfuel;

            pout = Pin * eps;
            tout = CalculateTemperatureOut(pout, Tin, eps, m, z);
            if (double.IsInfinity(tout))
            {
                result = GasCompressorUnitResultEnum.NOT_TEMPERATURE;
                tout = Tin;
            }

            if (n_otn < RevsRelativeMin)
            {
                result = GasCompressorUnitResultEnum.MIN_REVOLVES;
                CurrentPenalty += PenaltyCoeffPowerMax * (RevsRelativeMin - n_otn);
            }

            if (n_otn > RevsRelativeMax)
            {
                result = GasCompressorUnitResultEnum.MAX_REVOLVES;
                CurrentPenalty += PenaltyCoeffPowerMax * (n_otn - RevsRelativeMax);
            }

            return result;
        }

        public GasCompressorUnitResultEnum GetQMinQmax(double pin, double tin, out double qmin, out double qmax)
        {
            //Приводим к нужным величинам оперативные параметры
            double n_otn = N / NominalRevs; //Относительные обороты
            //Расчитываем приведенный расход
            double z = FlowProps.GetZ(pin, tin, ZMethod);
            double zs = FlowProps.GetZ(FlowProps.PStandart, FlowProps.TStandart, ZMethod);
            double ros = FlowProps.Ro;//GetStandartRo(M, zs);
            double ro = ros * FlowProps.TStandart * pin / (tin * FlowProps.PStandart * z);
            qmin = 1.100000001 * QReducedMin * ro * n_otn / (694.444 * ros);
            qmax = 0.99999 * QReducedMax * ro * n_otn / (694.444 * ros);
            var res = Calculate(pin, qmin, tin);
            if (res != GasCompressorUnitResultEnum.NORMAL)
            {
                return res;
            }
            res = Calculate(pin, qmax, tin);
            if (res == GasCompressorUnitResultEnum.NORMAL)
                return GasCompressorUnitResultEnum.NORMAL;
            //Здесь методов деления отрезка попалам мы ищем верхнюю границу расхода
            double qmaxn0 = qmin;
            double qmaxn1 = qmax;
            int iterCount = 10000;
            for (int i = 0; i < iterCount; i++)
            {
                double qavg = (qmaxn1 + qmaxn0) / 2;
                res = Calculate(pin, qavg, tin);
                if (res == GasCompressorUnitResultEnum.NORMAL)
                {
                    qmaxn0 = qavg;
                }
                else
                {
                    qmaxn1 = qavg;
                }
                if (Math.Abs(qmaxn0 - qmaxn1) < 0.000001)
                {
                    break;
                }
            }
            res = Calculate(pin, qmaxn0, tin);
            qmax = qmaxn0;
            return res;
        }

        public void GetQMinQmaxAll(double pin, double tin, out double qmin, out double qmax)
        {
            //Приводим к нужным величинам оперативные параметры
            double n_otn = N / NominalRevs; //Относительные обороты
            //Расчитываем приведенный расход
            double z = FlowProps.GetZ(pin, tin, ZMethod);
            double zs = FlowProps.GetZ(FlowProps.PStandart, FlowProps.TStandart, ZMethod);
            double ros = FlowProps.Ro;//GetStandartRo(M, zs);
            double ro = ros * FlowProps.TStandart * pin / (tin * FlowProps.PStandart * z);
            qmin = QReducedMin * ro * n_otn / (694.444 * ros);
            qmax = QReducedMax * ro * n_otn / (694.444 * ros);
        }

        #endregion

        #region Приватные методы

        public double GetKPD(double qpriv, double coefficient)
        {
            double res = 0;
            for (int i = 0; i < KPDKoefficients.Count; i++)
            {
                res += KPDKoefficients[i] * Math.Pow(qpriv, i);
            }
            return coefficient * res;
        }

        public double GetPower(double qpriv)
        {
            double res = 0;
            for (int i = 0; i < PowerKoefficients.Count; i++)
            {
                res += PowerKoefficients[i] * Math.Pow(qpriv, i);
            }
            return res;
        }

        public double GetKa(double P, double T, double ros)
        {
            double k = 1.556 - 3.9 * Math.Pow(10, -4) * T - 0.208 * ros + Math.Pow(P / T, 1.43) * (384 * Math.Pow(P / T, 0.8));
            return k;
        }

        private GasCompressorUnitResultEnum Calculate(double Pin, double Qin, double Tin)
        {
            double pout, tout, qfuel;
            return Calculate(Pin, Qin, Tin, out pout, out tout, out qfuel);
        }

        private double CalculateTemperatureOut(double pout, double tin, double eps, double m, double z)
        {
            double a = tin + 30;
            double b = tin + 20;

            //Метод секущих из библиотеки
            double tout;
            SecantMethod secantMethod = new SecantMethod();
            var hasRoot = secantMethod.Solve(new Func<double, double>(x =>
            {
                double zout = FlowProps.GetZ(pout, x, ZMethod);
                return x - tin * z / zout * Math.Pow(eps, (m - 1) / m);
            }), a, b, 0.0001, 1000, out tout);

            if (!hasRoot)
            {
                tout = double.PositiveInfinity;
            }

            return tout;
        }

        #endregion
    }
}
