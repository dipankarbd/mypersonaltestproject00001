using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UdacityRoboCar
{
    public partial class LocalizationGUI : Form
    {
        private Timer timer;
        protected Random random;

        private string[,] world;
        private float[,] p;

        int posX, posY;

        float sensor_right = 0.7f;
        float p_move = 0.8f;
        float sensor_wrong;
        float p_stay;


        public LocalizationGUI()
        {
            InitializeComponent();
            random = new Random();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            StartPosition = FormStartPosition.CenterScreen;

            timer = new Timer();
            timer.Tick += timer_Tick;
            timer.Interval = 60;
            timer.Start();

            InitLocalization();
        }

        public void InitLocalization()
        {
            world = new string[10, 10];
            p = new float[10, 10];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    p[i, j] = 1.0f / 100.0f;
                    world[i, j] = GetRandomColor();
                }
            }

            posX =  random.Next(0, 9);
            posY =  random.Next(0, 9);

            sensor_wrong = 1.0f - sensor_right;
            p_stay = 1.0f - p_move;

            //this.p = this.MoveRobot(p, new[] { 0, 0 });
            //this.p = this.Sense(p, this.world, GetMeasurement(posX, posY));
        }

        private string GetRandomColor()
        {
            if (random.NextDouble() > 0.5)
            {
                return "green";
            }
            else
            {
                return "red";
            }
        }

        private float[,] Sense(float[,] p, string[,] colors, string measurement)
        {
            float[,] aux = new float[p.GetLength(0), p.GetLength(1)];

            float s = 0.0f;
            for (int i = 0; i < p.GetLength(0); i++)
            {
                for (int j = 0; j < p.GetLength(1); j++)
                {
                    bool hit = measurement.Equals(colors[i, j]);
                    if (hit) aux[i, j] = p[i, j] * sensor_right;
                    else aux[i, j] = p[i, j] * sensor_wrong;
                    s += aux[i, j];
                }
            }
            for (int i = 0; i < aux.GetLength(0); i++)
            {
                for (int j = 0; j < aux.GetLength(1); j++)
                {
                    aux[i, j] = aux[i, j] / s;
                }
            }
            return aux;
        }

        private float[,] MoveRobot(float[,] p, int[] motion)
        {
            float[,] aux = new float[p.GetLength(0), p.GetLength(1)];
            for (int i = 0; i < p.GetLength(0); i++)
            {
                for (int j = 0; j < p.GetLength(1); j++)
                {
                    int x = (i - motion[0]) % p.GetLength(0);
                    if (x < 0) x = p.GetLength(0) + x;
                    int y = (j - motion[1]) % p.GetLength(1);
                    if (y < 0) y = p.GetLength(1) + y;

                    aux[i, j] = (p_move * p[x, y]) + (p_stay * p[i, j]);
                }
            }
            return aux;
        }


        void timer_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, this.Width, this.Height));

            int worldStartX = 10;
            int worldStartY = 10;

            Pen blackpen = new Pen(Brushes.Black, 1);
            Brush redBrush = new SolidBrush(Color.FromArgb(50, 255, 0, 0));
            Brush greenBrush = new SolidBrush(Color.FromArgb(20, 0, 255, 0));
            Image robotImage = Image.FromFile("robot.png");

            int startX = worldStartX;
            int startY = worldStartY;
            for (int i = 0; i < world.GetLength(0); i++)
            {
                startX = worldStartX;
                for (int j = 0; j < world.GetLength(1); j++)
                {
                    if (world[i, j] == "red")
                    {
                        e.Graphics.FillRectangle(redBrush, new Rectangle(startX, startY, 50, 50));
                    }
                    else if (world[i, j] == "green")
                    {
                        e.Graphics.FillRectangle(greenBrush, new Rectangle(startX, startY, 50, 50));
                    }
                    e.Graphics.DrawRectangle(blackpen, new Rectangle(startX, startY, 50, 50));

                    if (j == posX && i == posY)
                    {
                        e.Graphics.DrawImageUnscaled(robotImage, startX + (25 - robotImage.Width / 2), startY + (25 - robotImage.Height / 2), robotImage.Width, robotImage.Height);
                    }
                    startX += 50;
                }
                startY += 50;
            }

            //draw belife probability
            int pStartX = startX + 100;
            int pStartY = 10;

            startY = pStartY;
            for (int i = 0; i < p.GetLength(0); i++)
            {
                startX = pStartX;
                for (int j = 0; j < p.GetLength(1); j++)
                {
                    int opacity = (int)Math.Ceiling(p[i, j] * 255);
                    Brush probabilityBrush = new SolidBrush(Color.FromArgb(opacity, Color.DarkBlue));
                    e.Graphics.FillRectangle(probabilityBrush, new Rectangle(startX, startY, 50, 50));
                    e.Graphics.DrawRectangle(blackpen, new Rectangle(startX, startY, 50, 50));
                    startX += 50;
                }
                startY += 50;
            }
        }

        private void LocalizationGUI_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    posY = posY + 1;
                    if (posY > 9) posY = 0;
                    this.p = this.MoveRobot(p, new[] { 1, 0 });
                    this.p = this.Sense(p, this.world, GetMeasurement(posX, posY));
                    break;
                case Keys.Up:
                    posY = posY - 1;
                    if (posY < 0) posY = 9;
                    this.p = this.MoveRobot(p, new[] { -1, 0 });
                    this.p = this.Sense(p, this.world, GetMeasurement(posX, posY));
                    break;
                case Keys.Left:
                    posX = posX - 1;
                    if (posX < 0) posX = 9;
                    this.p = this.MoveRobot(p, new[] { 0, -1 });
                    this.p = this.Sense(p, this.world, GetMeasurement(posX, posY));
                    break;
                case Keys.Right:
                    posX = posX + 1;
                    if (posX > 9) posX = 0;
                    this.p = this.MoveRobot(p, new[] { 0, 1 });
                    this.p = this.Sense(p, this.world, GetMeasurement(posX, posY));
                    break;
                default:
                    break;
            }
        }

        private string GetMeasurement(int posX, int posY)
        {
            Console.WriteLine(this.world[posY, posX]);
            return this.world[posY, posX];
        }
    }
}
