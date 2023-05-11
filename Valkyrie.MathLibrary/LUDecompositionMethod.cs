namespace Valkyrie.MathLibrary
{
    /// <summary>
    /// Метод LU разложения матрицы и решения СЛАУ
    /// </summary>
    public class LUDecompositionMethod
    {
        #region Приватные свойства

        /// <summary>
        /// Матрица LU-декомопзиции
        /// Матрицы L и U хранятся в одной матрице и разделены клавной диагональю
        /// </summary>
        double[,] _matrixLU;

        /// <summary>
        /// 
        /// </summary>
        double[] _tempVector;

        double[] _result;

        #endregion

        #region Конструктор

        public LUDecompositionMethod(int n)
        {
            _matrixLU = new double[n, n];
            _result = new double[n];
            _tempVector = new double[n];
            N = n;
        }

        #endregion

        #region Свойства

        public int N
        {
            get;
            private set;
        }

        public double[,] LU
        {
            get;
            private set;
        }

        public double[] X
        {
            get
            {
                return _result;
            }
        }

        #endregion

        #region Методы

        public double[] Calculate(double[,] A, double[] b)
        {
            CreateMatrixLU(A, b);
            CreateSolve(A, b);


            return _result;
        }

        #endregion

        #region Приватные методы

        private void CreateMatrixLU(double[,] A, double[] b)
        {
            for (int j = 0; j < N; j++)
            {
                _matrixLU[0, j] = A[0, j];
                if (j > 0)
                    _matrixLU[j, 0] = A[j, 0] / _matrixLU[0, 0];
            }

            for (int i = 1; i < N; i++)
            {
                for (int j = i; j < N; j++)
                {
                    _matrixLU[i, j] = A[i, j];
                    for (int k = 0; k < i; k++)
                    {
                        _matrixLU[i, j] -= _matrixLU[i, k] * _matrixLU[k, j];
                    }
                    if (j > i)
                    {
                        _matrixLU[j, i] = A[j, i];
                        for (int k = 0; k < i; k++)
                        {
                            _matrixLU[j, i] -= _matrixLU[j, k] * _matrixLU[k, i];
                        }
                        _matrixLU[j, i] /= _matrixLU[i, i];
                    }
                }
            }

            //Console.WriteLine("---A---");
            //for (int i = 0; i < N; i++)
            //{
            //    for (int j = 0; j < N; j++)
            //    {
            //        Console.Write("{0:0.000}\t", A[i, j]);
            //    }
            //    Console.Write("{0:0.000}\t\t", b[i]);
            //    Console.WriteLine();
            //}

            //Console.WriteLine("---L---");
            //for (int i = 0; i < N; i++)
            //{
            //    for (int j = 0; j < N; j++)
            //    {
            //        if (i == j)
            //            Console.Write("1.000\t");
            //        else if (j < i + 1)
            //            Console.Write("{0:0.000}\t", _matrixLU[i, j]);
            //        else
            //            Console.Write("0.000\t");
            //    }
            //    Console.WriteLine();
            //}

            //Console.WriteLine("---U---");
            //for (int i = 0; i < N; i++)
            //{
            //    for (int j = 0; j < N; j++)
            //    {
            //        if (j >= i)
            //            Console.Write("{0:0.000}\t", _matrixLU[i, j]);
            //        else
            //            Console.Write("0.000\t");
            //    }
            //    Console.WriteLine();
            //}
        }

        private void CreateSolve(double[,] A, double[] b)
        {
            for (int i = 0; i < N; i++)
            {
                double res = 0;
                for (int j = 0; j < i; j++)
                {
                    res += _tempVector[j] * _matrixLU[i, j];
                }
                _tempVector[i] = (b[i] - res);
            }
            for (int i = b.Length - 1; i >= 0; i--)
            {
                double res = 0;
                for (int j = i + 1; j < N; j++)
                {
                    res += _result[j] * _matrixLU[i, j];
                }
                _result[i] = (_tempVector[i] - res) / _matrixLU[i, i];
            }

            //Console.WriteLine("---A---");
            //for (int i = 0; i < N; i++)
            //{
            //    for (int j = 0; j < N; j++)
            //    {
            //        Console.Write("{0:0.000}\t", A[i, j]);
            //    }
            //    Console.Write("{0:0.000}\t\t", b[i]);
            //    Console.WriteLine();
            //}

            //for (int i = 0; i < N; i++)
            //{
            //    Console.WriteLine("{0:0.000}", _result[i]);
            //}
        }

        #endregion
    }
}
