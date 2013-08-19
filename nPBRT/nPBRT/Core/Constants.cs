using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public  static class Constants
    {
        public static readonly double INV_PI;
        public static readonly double INV_TWOPI;
        public static readonly double INV_FOURPI;

        static Constants()
        {
            Console.WriteLine("ctr");
            INV_PI = 1 / (1 * Math.PI);
            INV_TWOPI = 1 / (2 * Math.PI);
            INV_FOURPI = 1 / (4 * Math.PI);
        }

    }
}
