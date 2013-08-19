using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public class Normal
    {
        public double x, y, z;
        public Normal()
        {
            this.x = this.y = this.z = 0.0d;
        }
        public Normal(double x, double y, double z)
        {
            if (HasNaNs()) throw new NotFiniteNumberException();

            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Normal(Vector v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }
        public bool HasNaNs()
        {
            return Double.IsNaN(this.x) || Double.IsNaN(this.x) || Double.IsNaN(this.x);
        }
        public static Normal operator -(Normal n)
        {
            return new Normal(-n.x, -n.y, -n.z);
        }
        public static Normal operator +(Normal n1, Normal n2)
        {
            return new Normal(n1.x + n2.x, n1.y + n2.y, n1.z + n2.z);
        }
        public static Normal operator -(Normal n1, Normal n2)
        {
            return new Normal(n1.x - n2.x, n1.y - n2.y, n1.z - n2.z);
        }
        public static Normal operator *(double f, Normal n)
        {
            return new Normal(f * n.x, f * n.y, f * n.z);
        }
        public static Normal operator *(Normal n, double f)
        {
            return new Normal(f * n.x, f * n.y, f * n.z);
        }
        public static Normal operator /(Normal n, double f)
        {
            if (f == 0.0d) throw new InvalidOperationException();
            double inv = 1.0d / f;
            return new Normal(n.x * inv, n.y * inv, n.z * inv);
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
            Normal n = obj as Normal;
            if ((System.Object)n == null)
            {
                return false;
            }

            return (this.x == n.x) && (this.y == n.y) && (this.z == n.z);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
