using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core.Geometry
{
    public class Geometry
    {
        public static float Dot(Vector v1, Vector v2)
        {
            if (v1.HasNaNs() || v2.HasNaNs()) throw new InvalidOperationException();
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }
        public static float AbsDot(Vector v1, Vector v2)
        {
            if (v1.HasNaNs() || v2.HasNaNs()) throw new InvalidOperationException();
            return Math.Abs(Dot(v1, v2));
        }
        public static Vector Cross(Vector v1, Vector v2)
        {
            if (v1.HasNaNs() || v2.HasNaNs()) throw new InvalidOperationException();
            return new Vector((v1.y * v2.z) - (v1.z * v2.y), (v1.z * v2.x) - (v1.x * v2.z), (v1.x * v2.y) - (v1.y * v2.x));
        }
        public static Vector Normalize(Vector v)
        {
            return v / v.Length();
        }
        public static Normal Normalize(Normal n)
        {
            return n / n.Length();
        }
        public static void CoordinateSystem(Vector v1, out Vector v2, out Vector v3)
        {
            if (Math.Abs(v1.x) > Math.Abs(v1.y))
            {
                float invLen = 1.0f / (float)Math.Sqrt(v1.x * v1.x + v1.z * v1.z);
                v2 = new Vector(-v1.z * invLen, 0.0f, v1.x * invLen);
            }
            else
            {
                float invLen = 1.0f / (float)Math.Sqrt(v1.y * v1.y + v1.z * v1.z);
                v2 = new Vector(0.0f, v1.z * invLen, -v1.y * invLen);
            }
            v3 = Cross(v1, v2);
        }

        public static float Distance(Point p1, Point p2)
        {
            return (p1 - p2).Length();
        }
        public static float DistanceSquared(Point p1, Point p2)
        {
            return (p1 - p2).LengthSquared();
        }

        public static float Dot(Normal n, Vector v)
        {
            if (n.HasNaNs() || v.HasNaNs()) throw new InvalidOperationException();
            return n.x * v.x + n.y * v.y + n.z * v.z;
        }
        public static float Dot(Vector v, Normal n)
        {
            if (v.HasNaNs() || n.HasNaNs()) throw new InvalidOperationException();
            return v.x * n.x + v.y * n.y + v.z * n.z;
        }
        public static float Dot(Normal n1, Normal n2)
        {
            if (n1.HasNaNs() || n2.HasNaNs()) throw new InvalidOperationException();
            return n1.x * n2.x + n1.y * n2.y + n1.z * n2.z;
        }


        public static float AbsDot(Normal n, Vector v)
        {
            if (n.HasNaNs() || v.HasNaNs()) throw new InvalidOperationException();
            return Math.Abs(n.x * v.x + n.y * v.y + n.z * v.z);
        }

        public static float AbsDot(Vector v, Normal n)
        {
            if (v.HasNaNs() || n.HasNaNs()) throw new InvalidOperationException();
            return Math.Abs(v.x * n.x + v.y * n.y + v.z * n.z);
        }
        public static float AbsDot(Normal n1, Normal n2)
        {
            if (n1.HasNaNs() || n2.HasNaNs()) throw new InvalidOperationException();
            return Math.Abs(n1.x * n2.x + n1.y * n2.y + n1.z * n2.z);
        }
        public static Normal Faceforward(Normal n, Vector v)
        {
            return (Dot(n, v) < 0.0f) ? -n : n;
        }
        public static Normal Faceforward(Normal n1, Normal n2)
        {
            return (Dot(n1, n2) < 0.0f) ? -n1 : n1;
        }
        public static Vector Faceforward(Vector v1, Vector v2)
        {
            return (Dot(v1, v2) < 0.0f) ? -v1 : v1;
        }
        public static Vector Faceforward(Vector v, Normal n)
        {
            return (Dot(v, n) < 0.0f) ? -v : v;
        }
        public static BBox Union(BBox b, Point p)
        {
            BBox ret = new BBox();
            ret.pMin.x = Math.Min(b.pMin.x, p.x);
            ret.pMin.y = Math.Min(b.pMin.y, p.y);
            ret.pMin.z = Math.Min(b.pMin.z, p.z);
            ret.pMax.x = Math.Max(b.pMax.x, p.x);
            ret.pMax.y = Math.Max(b.pMax.y, p.y);
            ret.pMax.z = Math.Max(b.pMax.z, p.z);
            return ret;
        }
        public static BBox Union(BBox b, BBox b2)
        {
            BBox ret = new BBox();
            ret.pMin.x = Math.Min(b.pMin.x, b2.pMin.x);
            ret.pMin.y = Math.Min(b.pMin.y, b2.pMin.y);
            ret.pMin.z = Math.Min(b.pMin.z, b2.pMin.z);
            ret.pMax.x = Math.Max(b.pMax.x, b2.pMax.x);
            ret.pMax.y = Math.Max(b.pMax.y, b2.pMax.y);
            ret.pMax.z = Math.Max(b.pMax.z, b2.pMax.z);
            return ret;
        }

        public static Vector SphericalDirection(float sintheta,
                                 float costheta, float phi)
        {
            return new Vector(sintheta * (float)Math.Cos(phi),
                          sintheta * (float)Math.Sin(phi),
                          costheta);
        }


        public static Vector SphericalDirection(float sintheta, float costheta,
                                         float phi, Vector x,
                                           Vector y, Vector z)
        {
            return sintheta * (float)Math.Cos(phi) * x +
                   sintheta * (float)Math.Sin(phi) * y + costheta * z;
        }


        public static float SphericalTheta(Vector v)
        {
            return (float)Math.Acos(Utility.Clamp(v.z, -1.0f, 1.0f));
        }


        public static float SphericalPhi(Vector v)
        {
            float p = (float)Math.Atan2(v.y, v.x);
            return (float)((p < 0.0f) ? p + 2.0f * Math.PI : p);
        }

    }
}
