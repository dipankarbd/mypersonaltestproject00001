using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public class Quaternion
    {
        public Vector v;
        public double w;

        public Quaternion()
        {
            v = new Vector(0.0d, 0.0d, 0.0d); w = 1.0d;
        }
        public Quaternion(Vector v, double w)
        {
            this.v = v;
            this.w = w;
        }
        public Quaternion(Transform t)
        {
            Matrix4x4 m = t.M;
            double trace = m.m[0, 0] + m.m[1, 1] + m.m[2, 2];
            if (trace > 0.0d)
            {
                // Compute w from matrix trace, then xyz
                // 4w^2 = m[0,0] + m[1,1] + m[2,2] + m[3,3] (but m[3,3] == 1)
                double s = Math.Sqrt(trace + 1.0d);
                w = s / 2.0d;
                s = 0.5f / s;
                v.x = (m.m[2, 1] - m.m[1, 2]) * s;
                v.y = (m.m[0, 2] - m.m[2, 0]) * s;
                v.z = (m.m[1, 0] - m.m[0, 1]) * s;
            }
            else
            {
                // Compute largest of $x$, $y$, or $z$, then remaining components
                int[] nxt = { 1, 2, 0 };
                double[] q = new double[3];
                int i = 0;
                if (m.m[1, 1] > m.m[0, 0]) i = 1;
                if (m.m[2, 2] > m.m[i, i]) i = 2;
                int j = nxt[i];
                int k = nxt[j];
                double s =  Math.Sqrt((m.m[i, i] - (m.m[j, j] + m.m[k, k])) + 1.0);
                q[i] = s * 0.5f;
                if (s != 0.0d) s = 0.5f / s;
                w = (m.m[k, j] - m.m[j, k]) * s;
                q[j] = (m.m[j, i] + m.m[i, j]) * s;
                q[k] = (m.m[k, i] + m.m[i, k]) * s;
                v.x = q[0];
                v.y = q[1];
                v.z = q[2];
            }
        }
        public Quaternion Slerp(double t, Quaternion q1,  Quaternion q2)
        {
            double cosTheta = Dot(q1, q2);
            if (cosTheta > .9995f)
                return Normalize((1.0d - t) * q1 + t * q2);
            else
            {
                double theta =  Math.Acos(Utility.Clamp(cosTheta, -1.0d, 1.0d));
                double thetap = theta * t;
                Quaternion qperp = Normalize(q2 - q1 * cosTheta);
                return q1 *  Math.Cos(thetap) + qperp *  Math.Sin(thetap);
            }
        }


        public static Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            Quaternion q = new Quaternion();
            q.v = q1.v + q2.v;
            q.w = q1.w + q2.w;
            return q;
        }
        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            Quaternion q = new Quaternion();
            q.v = q1.v - q2.v;
            q.w = q1.w - q2.w;
            return q;
        }
        public static Quaternion operator *(Quaternion q, double f)
        {
            Vector _v = q.v * f;
            double _w = q.w * f;
            return new Quaternion(_v, _w);
        }
        public static Quaternion operator *(double f, Quaternion q)
        {
            return q * f;
        }
        public static Quaternion operator /(Quaternion q, double f)
        {
            Vector _v = q.v / f;
            double _w = q.w / f;
            return new Quaternion(_v, _w);
        }

        public double Dot(Quaternion q1, Quaternion q2)
        {
            return  Geometry.Dot(q1.v, q2.v) + q1.w * q2.w;
        }


        public Quaternion Normalize(Quaternion q)
        {
            return q / Math.Sqrt(Dot(q, q));
        }
        public Transform ToTransform()
        {
            double xx = v.x * v.x, yy = v.y * v.y, zz = v.z * v.z;
            double xy = v.x * v.y, xz = v.x * v.z, yz = v.y * v.z;
            double wx = v.x * w, wy = v.y * w, wz = v.z * w;

            Matrix4x4 m = new Matrix4x4();
            m.m[0, 0] = 1.0d - 2.0d * (yy + zz);
            m.m[0, 1] = 2.0d * (xy + wz);
            m.m[0, 2] = 2.0d * (xz - wy);
            m.m[1, 0] = 2.0d * (xy - wz);
            m.m[1, 1] = 1.0d - 2.0d * (xx + zz);
            m.m[1, 2] = 2.0d * (yz + wx);
            m.m[2, 0] = 2.0d * (xz + wy);
            m.m[2, 1] = 2.0d * (yz - wx);
            m.m[2, 2] = 1.0d - 2.0d * (xx + yy);

            // Transpose since we are left-handed.  Ugh.
            return new Transform(Transform.Transpose(m), m);
        }

    }
}
