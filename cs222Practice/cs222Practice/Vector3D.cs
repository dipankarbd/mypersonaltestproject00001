using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs222Practice
{
    public class Vector3D
    {
        public double x, y, z;
        public Vector3D()
        {
            this.x = this.y = this.z = 0.0d;
        }
        public Vector3D(double x, double y, double z)
        {
            if (HasNaNs()) throw new NotFiniteNumberException();

            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool HasNaNs()
        {
            return Double.IsNaN(this.x) || Double.IsNaN(this.y) || Double.IsNaN(this.z);
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        public static Vector3D operator *(double f, Vector3D v)
        {
            return new Vector3D(f * v.x, f * v.y, f * v.z);
        }
        public static Vector3D operator *(Vector3D v, double f)
        {
            return new Vector3D(f * v.x, f * v.y, f * v.z);
        }
        public static Vector3D operator /(Vector3D v, double f)
        {
            if (f == 0.0d) throw new InvalidOperationException();
            double inv = 1.0d / f;
            return new Vector3D(v.x * inv, v.y * inv, v.z * inv);
        }
        public static Vector3D operator -(Vector3D v)
        {
            return new Vector3D(-v.x, -v.y, -v.z);
        }
        public double LengthSquared()
        {
            return this.x * this.x + this.y * this.y + this.z * this.z;
        }
        public double Length()
        {
            return Math.Sqrt(LengthSquared());
        }
        public double this[int index]
        {
            get
            {
                if (index < 0 || index > 2) throw new ArgumentException();

                if (index == 0) return x;
                else if (index == 1) return y;
                else if (index == 2) return z;
                else return 0;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Vector3D v = obj as Vector3D;
            if ((System.Object)v == null)
            {
                return false;
            }

            return (this.x == v.x) && (this.y == v.y) && (this.z == v.z);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
