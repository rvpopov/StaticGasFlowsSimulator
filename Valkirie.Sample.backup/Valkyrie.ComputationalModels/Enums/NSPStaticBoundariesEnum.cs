namespace Valkyrie.ComputationalModels.Enums
{
    /// <summary>
    /// Граничные условия для стационарного режима трубопровода
    /// </summary>
    public enum NSPStaticBoundariesEnum
    {
        /// <summary>
        /// Заданы давления на входе и на выходе
        /// </summary>
        P0_PL,

        /// <summary>
        /// Заданы давление на входе и расход
        /// </summary>
        P0_Q,

        /// <summary>
        /// Заданы давления на выходе и расход
        /// </summary>
        PL_Q
    }
}
