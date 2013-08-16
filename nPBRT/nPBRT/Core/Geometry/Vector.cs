using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core.Geometry
{
    public class Vector
    {
        public double x, y, z;
        public Vector()
        {
            this.x = this.y = this.z = 0.0d;
        }
        public Vector(double x, double y, double z)
        {
            if (HasNaNs()) throw new NotFiniteNumberException();

            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector(Normal v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }
        public bool HasNaNs()
        {
            return Double.IsNaN(this.x) || Double.IsNaN(this.x) || Double.IsNaN(this.x);
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        public static Vector operator *(double f, Vector v)
        {
            return new Vector(f * v.x, f * v.y, f * v.z);
        }
        public static Vector operator *(Vector v, double f)
        {
            return new Vector(f * v.x, f * v.y, f * v.z);
        }
        public static Vector operator /(Vector v, double f)
        {
            if (f == 0.0d) throw new InvalidOperationException();
            double inv = 1.0d / f;
            return new Vector(v.x * inv, v.y * inv, v.z * inv);
        }
        public static Vector operator -(Vector v)
        {
            return new Vector(-v.x, -v.y, -v.z);
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
            Vector v = obj as Vector;
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
