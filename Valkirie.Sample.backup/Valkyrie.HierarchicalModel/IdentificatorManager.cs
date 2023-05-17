namespace Valkyrie.HierarchicalModel
{
    /// <summary>
    /// Статический менеджер идентификаторов объектов
    /// </summary>
    public static class IdentificatorManager
    {
        /// <summary>
        /// Текущее значение счетчика
        /// </summary>
        private static int _currentValue = 0;

        /// <summary>
        /// Возвращает новое значение счетчика идентификаторов
        /// </summary>
        /// <returns>Новое значение счетчика идентификаторов</returns>
        public static int GetNextValue()
        {
            return ++_currentValue;
        }

        /// <summary>
        /// Возвращает текущее значение счетчика идентификаторов
        /// </summary>
        /// <returns>Текущее значение счетчика идентификаторов</returns>
        public static int GetCurrentValue()
        {
            return _currentValue;
        }

        /// <summary>
        /// Сбрасывает счетчик идентификаторов
        /// </summary>
        public static void Reset()
        {
            _currentValue = 0;
        }
    }
}
