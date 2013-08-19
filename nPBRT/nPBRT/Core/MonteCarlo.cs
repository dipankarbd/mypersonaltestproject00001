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

        public static void ConcentricSampleDisk(double u1, double u2, ref double dx, ref double dy)
        {
            double r, theta;
            // Map uniform random numbers to $[-1,1]^2$
            double sx = 2 * u1 - 1;
            double sy = 2 * u2 - 1;

            // Map square to $(r,\theta)$

            // Handle degeneracy at the origin
            if (sx == 0.0 && sy == 0.0)
            {
                dx = 0.0;
                dy = 0.0;
                return;
            }
            if (sx >= -sy)
            {
                if (sx > sy)
                {
                    // Handle first region of disk
                    r = sx;
                    if (sy > 0.0) theta = sy / r;
                    else theta = 8.0d + sy / r;
                }
                else
                {
                    // Handle second region of disk
                    r = sy;
                    theta = 2.0d - sx / r;
                }
            }
            else
            {
                if (sx <= sy)
                {
                    // Handle third region of disk
                    r = -sx;
                    theta = 4.0d - sy / r;
                }
                else
                {
                    // Handle fourth region of disk
                    r = -sy;
                    theta = 6.0d + sx / r;
                }
            }
            theta *= Math.PI / 4.0d;
            dx = r * Math.Cos(theta);
            dy = r * Math.Sin(theta);
        }
    }
}
