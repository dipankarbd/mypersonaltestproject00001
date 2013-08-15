using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public class Utility
    {
        public static float Lerp(float t, float v1, float v2)
        {
            return (1.0f - t) * v1 + t * v2;
        }
        public static void Swap<T>(ref T left, ref T right)
        {
            T tmp = left;
            left = right;
            right = tmp;
        }
        public static float Clamp(float val, float low, float high)
        {
            if (val < low) return low;
            else if (val > high) return high;
            else return val;
        }
        public static float Radians(float deg)
        {
            return ((float)Math.PI / 180.0f) * deg;
        }
    }
}
