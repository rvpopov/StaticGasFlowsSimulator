using Valkyrie.ComputationalModels.Enums;

namespace Valkyrie.ComputationalModels.Models
{
    public class ValveModel
    {
        #region Конструктор

        public ValveModel()
        {
            State = ValveStates.Close;
        }

        #endregion

        #region Свойства

        public ValveStates State
        {
            get;
            set;
        }

        #endregion
    }
}
