using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs222Practice
{
    public class Unit3
    {
        private double h = 0.5;// days
        private double transmissionCoeff = 5e-9;// 1 / day person
        private double latencyTime = 1.0;// days
        private double infectiousTime = 5.0;// days

        private double endTime = 60.0;// days

        private int numSteps;
        private double[] times;

        public Unit3()
        {
            Part1();
        }

        private void Part1()
        {
            numSteps = (int)(endTime / h);
            times = new double[numSteps + 1];
            for (int j = 0; j < times.Length; j++)
            {
                times[j] = h * j;
            }

            double[] s, e, i, r;
            SEIRModel(out s, out e, out i, out r);
            StringBuilder sb = new StringBuilder();
            sb.Append("t,s,e,i,r " + Environment.NewLine);
            for (int step = 0; step < numSteps + 1; step++)
            {
                sb.Append(times[step] + "," + s[step] + "," + e[step] + "," + i[step] + "," + r[step] + Environment.NewLine);
            }
            File.WriteAllText("seir_programming.csv", sb.ToString());
            Console.WriteLine("done...");
        }

        private void SEIRModel(out double[] s, out double[] e, out double[] i, out double[] r)
        {
            s = new double[numSteps + 1];
            e = new double[numSteps + 1];
            i = new double[numSteps + 1];
            r = new double[numSteps + 1];
            for (int j = 0; j < numSteps + 1; j++)
            {
                s[j] = 0.0;
                e[j] = 0.0;
                i[j] = 0.0;
                r[j] = 0.0;
            }

            s[0] = 1e8 - 1e6 - 1e5;
            e[0] = 0.0;
            i[0] = 1e5;
            r[0] = 1e6;

            for (int step = 0; step < numSteps; step++)
            {
                double s2e = h * transmissionCoeff * s[step] * i[step];
                double e2i = h * e[step] / latencyTime;
                double i2r = h * i[step] / infectiousTime;

                s[step + 1] = s[step] - s2e;
                e[step + 1] = e[step] + s2e - e2i;
                i[step + 1] = i[step] + e2i - i2r;
                r[step + 1] = r[step] + i2r;
            }

        }
    }
}
