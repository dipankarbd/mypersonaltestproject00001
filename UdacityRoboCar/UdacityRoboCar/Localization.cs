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
}
