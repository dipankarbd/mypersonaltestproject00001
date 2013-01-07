using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdacityRoboCar
{
    public class Localization
    {
        private float[] p = { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };
        private string[] world = { "green", "red", "red", "green", "green" };

        private string[] measurements = { "red", "red" };
        private int[] motions = { 1, 1 };

        private float pHit = 0.6f;
        private float pMiss = 0.2f;

        private float pExact = 0.8f;
        private float pOvershoot = 0.1f;
        private float pUndershoot = 0.1f;

        private float[] Sense(float[] p, string Z)
        {
            float[] q = new float[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                bool hit = Z.Equals(this.world[i]);
                if (hit) q[i] = p[i] * pHit;
                else q[i] = p[i] * pMiss;
            }

            float s = q.Sum();
            for (int i = 0; i < q.Length; i++)
            {
                q[i] = q[i] / s;
            }

            return q;
        }
        private float[] Move(float[] p, int U)
        {
            float[] q = new float[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                int indexExact = (i - U) % p.Length;
                int indexOver = (i - U - 1) % p.Length;
                int indexUnder = (i - U + 1) % p.Length;

                if (indexExact < 0) indexExact = p.Length + indexExact;
                if (indexOver < 0) indexOver = p.Length + indexOver;
                if (indexUnder < 0) indexUnder = p.Length + indexUnder;


                float s = pExact * p[indexExact];
                s = s + pOvershoot * p[indexOver];
                s = s + pUndershoot * p[indexUnder];
                q[i] = s;
            }
            return q;
        }

        public void Execute()
        {
            Console.WriteLine("**************************** LOCALIZATION**************************************");
            for (int i = 0; i < measurements.Length; i++)
            {
                p = Sense(p, measurements[i]);
                p = Move(p, motions[i]);
            }

            for (int i = 0; i < p.Length; i++)
            {
                Console.Write(p[i] + "\t");
            }
        }

    }

    public class Localization2
    {
        private string[,] colors = { { "red", "green", "green", "red", "red" }, { "red", "red", "green", "red", "red" }, { "red", "red", "green", "green", "red" }, { "red", "red", "red", "red", "red" } };
        private string[] measurements = { "green", "green", "green", "green", "green" };
        private int[,] motions = { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 0 }, { 0, 1 } };

        float sensor_right = 0.7f;
        float p_move = 0.8f;

        float sensor_wrong;
        float p_stay;
        private float[,] p;

        public Localization2()
        {
            sensor_wrong = 1.0f - sensor_right;
            p_stay = 1.0f - p_move;
        }

        public void Execute()
        {
            Console.WriteLine("**************************** LOCALIZATION 2**************************************");
            if (measurements.Length != motions.GetLength(0)) throw new Exception("error in size of measurement/motion vector");



            p = new float[colors.GetLength(0), colors.GetLength(1)];
            float pinit = 1.0f / (float)colors.Length;

            for (int i = 0; i < colors.GetLength(0); i++)
            {
                for (int j = 0; j < colors.GetLength(1); j++)
                {
                    p[i, j] = pinit;
                }
            }

            for (int i = 0; i < measurements.Length; i++)
            {
                p = this.Move(p, new[] { motions[i, 0], motions[i, 1] });
                p = this.Sense(p, colors, measurements[i]);
            }

            this.Show(p);
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

        private float[,] Move(float[,] p, int[] motion)
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


        public void Show(float[,] p)
        {
            for (int i = 0; i < p.GetLength(0); i++)
            {
                for (int j = 0; j < p.GetLength(1); j++)
                {
                    Console.Write(p[i, j] + "\t");
                }
                Console.Write("\n");
            }
        }
    }
}
