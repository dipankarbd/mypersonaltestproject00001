using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs222Practice
{
    public class Unit2
    {


        double totalTime = 24.0d * 3600.0d; //  s
        double g = 9.81;// m / s2
        double earthMass = 5.97e24;// kg
        double gravitationalConstant = 6.67e-11;// N m2 / kg2
        double radius;
        double speed;

        public Unit2()
        {
            Part1();
        }

        private void Part1()
        {
            radius = Math.Pow((gravitationalConstant * earthMass * Math.Pow(totalTime, 2) / 4.0d / Math.Pow(Math.PI, 2)), (1.0d / 3.0d));
            speed = 2.0 * Math.PI * radius / totalTime;

            StringBuilder sb = new StringBuilder();
            sb.Append("h,e" + Environment.NewLine);
            foreach (var numSteps in new int[] { 200, 500, 1000, 2000, 5000, 10000 })
            {
                double h, e;
                CalculateError(numSteps, out h, out e);
                sb.Append(h + "," + e + Environment.NewLine);
            }

            File.WriteAllText("orbit_errror.csv", sb.ToString());
        }


        private Vector2D Acceleration(Vector2D spaceshipPosition)
        {
            Vector2D vectorToEarth = -spaceshipPosition;//earth located at origin
            return gravitationalConstant * earthMass * vectorToEarth / Math.Pow(vectorToEarth.Length(), 3);
        }


        private void CalculateError(int numSteps, out double h, out double e)
        {
            h = totalTime / numSteps;
            Vector2D[] x = new Vector2D[numSteps + 1];
            Vector2D[] v = new Vector2D[numSteps + 1];

            for (int i = 0; i < numSteps + 1; i++)
            {
                x[i] = new Vector2D();
                v[i] = new Vector2D();
            }


            x[0].x = radius;
            v[0].y = speed;

            for (int step = 0; step < numSteps; step++)
            {
                x[step + 1] = x[step] + h * v[step];
                v[step + 1] = v[step] + h * Acceleration(x[step]);
            }
            e = (x[numSteps] - x[0]).Length();
        }
    }
}
