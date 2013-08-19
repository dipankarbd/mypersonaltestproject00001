using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public class MonteCarlo
    {
        public static Vector UniformSampleSphere(double u1, double u2)
        {
            double z = 1.0d - 2.0d * u1;
            double r = Math.Sqrt(Math.Max(0.0d, 1.0d - z * z));
            double phi = 2.0d * Math.PI * u2;
            double x = r * Math.Cos(phi);
            double y = r * Math.Sin(phi);
            return new Vector(x, y, z);
        }

        public static Vector UniformSampleCone(double u1, double u2, double costhetamax)
        {
            double costheta = (1.0d - u1) + u1 * costhetamax;
            double sintheta = Math.Sqrt(1.0d - costheta * costheta);
            double phi = u2 * 2.0d * Math.PI;
            return new Vector(Math.Cos(phi) * sintheta, Math.Sin(phi) * sintheta, costheta);
        }


        public static Vector UniformSampleCone(double u1, double u2, double costhetamax, Vector x, Vector y, Vector z)
        {
            double costheta = Utility.Lerp(u1, costhetamax, 1.0d);
            double sintheta = Math.Sqrt(1.0d - costheta * costheta);
            double phi = u2 * 2.0d * Math.PI;
            return Math.Cos(phi) * sintheta * x + Math.Sin(phi) * sintheta * y + costheta * z;
        }

        public static double UniformConePdf(double cosThetaMax)
        {
            return 1.0d / (2.0d * Math.PI * (1.0d - cosThetaMax));
        }
    }
}
