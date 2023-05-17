using System;

namespace Valkyrie.MathLibrary.Events
{
    public class IterationEventArgs : EventArgs
    {
        public double ObjVal
        {
            get;
            set;
        }

        public int Iter
        {
            get;
            set;
        }
    }
}
