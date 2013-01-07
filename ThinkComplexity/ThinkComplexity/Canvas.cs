using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThinkComplexity
{
    public partial class Canvas : Form
    {
        private Timer timer;
        protected Random random;
        public Canvas()
        {
            InitializeComponent();

            random = new Random();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            //FormBorderStyle = FormBorderStyle.FixedToolWindow;

            StartPosition = FormStartPosition.CenterScreen;

            timer = new Timer();

            timer.Tick += new EventHandler(this.timer_Tick);

            timer.Interval = 60;

            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
 
    }
}
