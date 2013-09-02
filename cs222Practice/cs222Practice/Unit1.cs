using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;

namespace cs222Practice
{
    public class Unit1
    {
        public Unit1()
        {
            double[] t,  x,  v; 
            ForwardEuler(out t,out  x, out v);

            Console.WriteLine("t\tx\tv");
            for (int i = 0; i < t.Length; i++)
            {
                Console.WriteLine(t[i].ToString("0.00") + "\t" + x[i].ToString("0.00") + "\t" + v[i].ToString("0.00"));
            }


            var pm = new PlotModel("Trigonometric functions", "Example using the FunctionSeries")
            {
                PlotType = PlotType.Cartesian,
                Background = OxyColors.White
            };

            LineSeries s1 = new LineSeries();
            LineSeries s2 = new LineSeries();
        
            for (int i = 0; i < t.Length; i++)
            {
                s1.Points.Add(new DataPoint(t[i],x[i]));
                s1.Points.Add(new DataPoint(t[i], v[i]));
            }
            pm.Series.Add(s1);
              
            GraphForm frm = new GraphForm(pm);
            frm.ShowDialog();
             
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
    }
}
