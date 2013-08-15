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
    }
}
