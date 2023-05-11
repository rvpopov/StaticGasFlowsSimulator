using System;

namespace Valkyrie.MathLibrary
{
    /// <summary>
    /// Метод секущих
    /// </summary>
    public class SecantMethod
    {
        public bool Solve(Func<double, double> equation, double a, double b, double accuracy, int maxIterations, out double x)
        {
            bool hasRoot = false;
            double f_a = equation(a);
            x = 0;
            if (Math.Abs(f_a) < accuracy)
            {
                x = a;
                return true;
            }

            for (int i = 0; i < maxIterations; i++)
            {
                var f_b = equation(b);
                if (Math.Abs(f_b) < accuracy)
                {
                    x = b;
                    hasRoot = true;
                    break;
                }

                var a1 = b - (b - a) * f_b / (f_b - f_a);
                var f_a1 = equation(a1);

                if (Math.Abs(f_a1) < accuracy)
                {
                    x = a1;
                    hasRoot = true;
                    break;
                }

                var b1 = a1 - (a1 - b) * f_a1 / (f_a1 - f_b);

                if (double.IsNaN(a1 + b1))
                    break;

                f_a = f_a1;
                a = a1;
                b = b1;
                //if (Math.Max(Math.Abs(f_a), Math.Abs(f_b)) < accuracy)
                //{
                //    hasRoot = true;
                //    break;
                //}
            }
            //x = (a + b) / 2;
            return hasRoot;
        }
    }
}
