
namespace Valkyrie.Sigryun
{
    /// <summary>
    /// Представляет интерфейс модели технологического объекта
    /// </summary>
    public interface IComputationalModel
    {
        /// <summary>
        /// Получает или задает тег объекта
        /// </summary>
        object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Признак того, что дуга должна быть удалена из графа
        /// </summary>
        bool IsDisabled
        {
            get;
        }

        /// <summary>
        /// Получает признак того, что дуга должна быть эквивалентирована в узел
        /// </summary>
        bool IsIgnorable
        {
            get;
        }

        /// <summary>
        /// Вызывает копирование объекта
        /// </summary>
        /// <returns></returns>
        IComputationalModel Clone();
    }
}
