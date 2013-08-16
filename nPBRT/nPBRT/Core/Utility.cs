using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public class Utility
    {
        public static double Lerp(double t, double v1, double v2)
        {
            return (1.0d - t) * v1 + t * v2;
        }
        public static void Swap<T>(ref T left, ref T right)
        {
            T tmp = left;
            left = right;
            right = tmp;
        }
        public static double Clamp(double val, double low, double high)
        {
            if (val < low) return low;
            else if (val > high) return high;
            else return val;
        }
        public static double Radians(double deg)
        {
            return (Math.PI / 180.0d) * deg;
        }

        public static bool SolveLinearSystem2x2(double[,] A, double[] B, out double x0, out double x1)
        {
            double det = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            if (Math.Abs(det) < 1e-10d)
            {
                x0 = x1 = 0.0d;
                return false;
            }
            x0 = (A[1, 1] * B[0] - A[0, 1] * B[1]) / det;
            x1 = (A[0, 0] * B[1] - A[1, 0] * B[0]) / det;
            if (Double.IsNaN(x0) || Double.IsNaN(x1))
                return false;
            return true;
        }
    }
}
