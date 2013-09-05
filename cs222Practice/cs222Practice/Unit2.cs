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

        
        double spacecraftMass = 30000.0d;// kg 

        List<double> hArray;
        List<double> eulerErrorArray;
        List<double> heunsErrorArray;

        public Unit2()
        {
            radius = Math.Pow((gravitationalConstant * earthMass * Math.Pow(totalTime, 2) / 4.0d / Math.Pow(Math.PI, 2)), (1.0d / 3.0d));
            speed = 2.0 * Math.PI * radius / totalTime;

            Part4();
        }

        private void Part1()
        {
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
        private void Part2()
        {
            hArray = new List<double>();
            eulerErrorArray = new List<double>();
            heunsErrorArray = new List<double>();

            foreach (var numSteps in new int[] { 50, 100, 200, 500, 1000 })
            {
                Vector2D[] x;
                Vector2D[] v;
                double error;
                HeunsMethod(numSteps, out x, out v, out error);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("step size,error(euler),error(heuns)" + Environment.NewLine);
            for (int i = 0; i < hArray.Count; i++)
            {
                sb.Append(hArray[i] + "," + eulerErrorArray[i] + "," + heunsErrorArray[i] + Environment.NewLine);
            }
            File.WriteAllText("programming_heuns.csv", sb.ToString());
        }


        private void Part3()
        {
            totalTime = 12500.0d;
            Vector2D x, v;
            Orbit(out x, out v);
            Console.WriteLine("done...");
        }

        private void Part4()
        {
            Vector2D[] x;
            double[] energy;
            TotalEnergy(out x, out energy);

            StringBuilder sb = new StringBuilder();
            sb.Append("i,x.x,x.y,e" + Environment.NewLine);
            for (int i = 0; i < x.Length; i++)
            {
                sb.Append(i + "," + x[i].x + "," + x[i].y + "," + energy[i] + Environment.NewLine);
            }
            File.WriteAllText("energy.csv", sb.ToString());
            Console.WriteLine("done...");
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

        private void HeunsMethod(int numSteps, out Vector2D[] x, out Vector2D[] v, out double error)
        {
            double h = totalTime / numSteps;
            x = new Vector2D[numSteps + 1];
            v = new Vector2D[numSteps + 1];
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

            error = (x[numSteps] - x[0]).Length();

            hArray.Add(h);
            eulerErrorArray.Add(error);

            //Heun's Method
            for (int step = 0; step < numSteps; step++)
            {
                Vector2D initialAcceleration = Acceleration(x[step]);
                Vector2D xE = x[step] + h * v[step];
                Vector2D vE = v[step] + h * initialAcceleration;

                x[step + 1] = x[step] + h * 0.5 * (v[step] + vE);
                v[step + 1] = v[step] + h * 0.5 * (initialAcceleration + Acceleration(xE));
            }

            error = (x[numSteps] - x[0]).Length();
            heunsErrorArray.Add(error);
        }

        private void Orbit(out Vector2D x, out Vector2D v)
        {
            x = new Vector2D(); // m
            v = new Vector2D(); // m / s
            x.x = 15e6;
            x.y = 1e6;
            v.x = 2e3;
            v.y = 4e3;

            StringBuilder sb = new StringBuilder();
            sb.Append("x.x,x.y,s" + Environment.NewLine);
            sb.Append(x.x + "," + x.y + "," + 4 + Environment.NewLine);

            double currentTime = 0.0d; // s
            double h = 100.0d;// s
            double hNew = h;// s, will store the adaptive step size of the next step
            double tolerance = 5e5;//m

            while (currentTime < totalTime)
            {
                Vector2D acceleration0 = Acceleration(x);
                Vector2D xE = x + h * v;
                Vector2D vE = v + h * acceleration0;
                Vector2D xH = x + h * 0.5 * (v + vE);
                Vector2D vH = v + h * 0.5 * (acceleration0 + Acceleration(xE));
                x = xH;
                v = vH;

                double error = (xE - xH).Length() + totalTime * ((vE - vH).Length());
                hNew = h * Math.Sqrt(tolerance / (error + 1e-50));
                hNew = Math.Min(1800.0d, Math.Max(0.1d, hNew));

                sb.Append(x.x + "," + x.y + "," + 1 + Environment.NewLine);
                currentTime += h;
                h = hNew;
            }

            sb.Append(0 + "," + 0 + "," + 0 + Environment.NewLine);

            File.WriteAllText("adaptive_step_size.csv", sb.ToString());
        }

        private void TotalEnergy(out Vector2D[] x, out double[] energy)
        {
            int numSteps = 20000;
            double h = 5.0;//s 

            x = new Vector2D[numSteps + 1]; // m
            Vector2D[] v = new Vector2D[numSteps + 1]; // m / s
            energy = new double[numSteps + 1];//  J = kg m2 / s2

            for (int i = 0; i < numSteps + 1; i++)
            {
                x[i] = new Vector2D();
                v[i] = new Vector2D();
            }

            x[0].x = 15e6;
            x[0].y = 1e6;
            v[0].x = 2e3;
            v[0].y = 4e3;

            for (int step = 0; step < numSteps; step++)
            {
                x[step + 1] = x[step] + h * v[step];
                v[step + 1] = v[step] + h * Acceleration(x[step]);
            }

            for (int step = 0; step < numSteps + 1; step++)
            {
                energy[step] = 0.5 * spacecraftMass * Math.Pow(v[step].Length(), 2) - gravitationalConstant * earthMass * spacecraftMass/x[step].Length();
            }

        }
    }
}
