using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace cs222Practice
{
    public partial class GraphForm : Form
    {
        public OxyPlot.WindowsForms.Plot Plot;
        public GraphForm()
        {
            InitializeComponent();

            var pm = new PlotModel("Trigonometric functions", "Example using the FunctionSeries")
            {
                PlotType = PlotType.Cartesian,
                Background = OxyColors.White
            };
            pm.Series.Add(new FunctionSeries(Math.Sin, -10, 10, 0.1, "sin(x)"));
            pm.Series.Add(new FunctionSeries(Math.Cos, -10, 10, 0.1, "cos(x)"));
            pm.Series.Add(new FunctionSeries(t => 5 * Math.Cos(t), t => 5 * Math.Sin(t), 0, 2 * Math.PI, 0.1, "cos(t),sin(t)"));
            plot1.Model = pm;
        }

        public GraphForm(PlotModel model)
        {
            InitializeComponent();

            plot1.Model = model;
        }
    }
}
