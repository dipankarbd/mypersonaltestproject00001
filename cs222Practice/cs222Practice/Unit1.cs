using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;

namespace cs222Practice
{
    public class Unit1
    {
        private double earthMass = 5.97e24;// kg
        private double moonMass = 7.35e22;//kg
        private double G = 6.67e-11;// N m2 / kg2

        public Unit1()
        {

            Part2();
        }


        public void Part1()
        {
            double[] t, x, v;
            ForwardEuler(out t, out  x, out v);

            StringBuilder sb = new StringBuilder();

            sb.Append("t,x,v" + Environment.NewLine);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i] + "," + x[i] + "," + v[i] + Environment.NewLine);
            }

            File.WriteAllText("eulerian_free_fall.csv", sb.ToString());

        }

        public void Part2()
        {
        }


        public void ForwardEuler(out double[] t, out double[] x, out double[] v)
        {
            double h = 0.1;
            double g = 9.81;

            int numSteps = 50;

            t = new double[numSteps + 1];
            x = new double[numSteps + 1];
            v = new double[numSteps + 1];


            for (int step = 0; step < numSteps; step++)
            {
                t[step + 1] = h * (step + 1);
                x[step + 1] = x[step] + h * v[step];
                v[step + 1] = v[step] - h * g;
            }
        }

        public Vector2D Acceleration(Vector2D moonPosition, Vector2D spaceshipPosition)
        {
            Vector2D vectorToEarth = -spaceshipPosition;
            Vector2D vectorToMoon = moonPosition - spaceshipPosition;

            Vector2D acc = G * (earthMass * vectorToEarth / Math.Pow(vectorToEarth.Length(), 3) + moonMass * vectorToMoon / Math.Pow(vectorToMoon.Length(), 3));
            return acc;
        }
    }
}
