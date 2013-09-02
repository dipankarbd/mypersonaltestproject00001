using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs222Practice
{
    public class Vector2D
    {
        public double x, y;
        public Vector2D()
        {
            this.x = this.y = 0.0d;
        }
        public Vector2D(double x, double y)
        {
            if (HasNaNs()) throw new NotFiniteNumberException();

            this.x = x;
            this.y = y;
        }

        public bool HasNaNs()
        {
            return Double.IsNaN(this.x) || Double.IsNaN(this.y);
        }

        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.x - v2.x, v1.y - v2.y);
        }
        public static Vector2D operator *(double f, Vector2D v)
        {
            return new Vector2D(f * v.x, f * v.y);
        }
        public static Vector2D operator *(Vector2D v, double f)
        {
            return new Vector2D(f * v.x, f * v.y);
        }
        public static Vector2D operator /(Vector2D v, double f)
        {
            if (f == 0.0d) throw new InvalidOperationException();
            double inv = 1.0d / f;
            return new Vector2D(v.x * inv, v.y * inv);
        }
        public static Vector2D operator -(Vector2D v)
        {
            return new Vector2D(-v.x, -v.y);
        }
        public double LengthSquared()
        {
            return this.x * this.x + this.y * this.y;
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
                else return 0;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Vector2D v = obj as Vector2D;
            if ((System.Object)v == null)
            {
                return false;
            }

            return (this.x == v.x) && (this.y == v.y);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
