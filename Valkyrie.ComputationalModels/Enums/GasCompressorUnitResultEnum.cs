namespace Valkyrie.Sigryun.Models.KompressorShopModels
{
    public enum GasCompressorUnitResultEnum
    {
        /// <summary>
        /// Превышен коэффициент удаленности
        /// </summary>
        KUD,

        /// <summary>
        /// Превышен коэффициент загрузки
        /// </summary>
        KLOAD,

        /// <summary>
        /// Превышен максимальный расход
        /// </summary>
        QMAX,

        /// <summary>
        /// Превышение номинальной мощности
        /// </summary>
        POWER,

        /// <summary>
        /// Нормальный расчет
        /// </summary>
        NORMAL,

        /// <summary>
        /// Нарушение орграничения по минимальным оборотам
        /// </summary>
        MIN_REVOLVES,

        /// <summary>
        /// Нарушение орграничения по максимальным оборотам
        /// </summary>
        MAX_REVOLVES,

        /// <summary>
        /// Ошибка при вычислении температуры нагнетания
        /// </summary>
        NOT_TEMPERATURE
    }
}
