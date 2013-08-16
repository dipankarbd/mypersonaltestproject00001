using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core.Geometry
{
    public class Ray
    {
        public Point o;
        public Vector d;
        public double mint, maxt;
        public double time;
        public int depth;

        public Ray()
        {
            this.mint = 0.0d;
            this.maxt = double.PositiveInfinity;
            this.time = 0.0d;
            this.depth = 0;
        }
        public Ray(Point origin, Vector direction, double start, double end = double.PositiveInfinity, double t = 0.0d, int d = 0)
        {
            this.o = origin;
            this.d = direction;
            this.mint = start;
            this.maxt = end;
            this.time = t;
            this.depth = d;
        }
        public Ray(Point origin, Vector direction, Ray parent, double start, double end = double.PositiveInfinity)
        {
            this.o = origin;
            this.d = direction;
            this.mint = start;
            this.maxt = end;
            this.time = parent.time;
            this.depth = parent.depth + 1;
        }
        public Point Value(double t)
        {
            return o + d * t;
        }
        public virtual bool HasNaNs()
        {
            return (o.HasNaNs() || d.HasNaNs() || Double.IsNaN(mint) || Double.IsNaN(maxt));
        }
        public void DoSomething()
        {
            this.o = new Point();
        }
        public Ray Copy()
        {
            Ray _r = new Ray();
            _r.o = this.o;
            _r.d = this.d;
            _r.mint = this.mint;
            _r.maxt = this.maxt;
            _r.time = this.time;
            _r.depth = this.depth;
            return _r;
        }
    }
}
