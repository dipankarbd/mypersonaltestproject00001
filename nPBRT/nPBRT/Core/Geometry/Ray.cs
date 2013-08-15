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
        public float mint, maxt;
        public float time;
        public int depth;

        public Ray()
        {
            this.mint = 0.0f;
            this.maxt = float.PositiveInfinity;
            this.time = 0.0f;
            this.depth = 0;
        }
        public Ray(Point origin, Vector direction, float start, float end = float.PositiveInfinity, float t = 0.0f, int d = 0)
        {
            this.o = origin;
            this.d = direction;
            this.mint = start;
            this.maxt = end;
            this.time = t;
            this.depth = d;
        }
        public Ray(Point origin, Vector direction, Ray parent, float start, float end = float.PositiveInfinity)
        {
            this.o = origin;
            this.d = direction;
            this.mint = start;
            this.maxt = end;
            this.time = parent.time;
            this.depth = parent.depth + 1;
        }
        public Point Value(float t)
        {
            return o + d * t;
        }
        public virtual bool HasNaNs()
        {
            return (o.HasNaNs() || d.HasNaNs() || float.IsNaN(mint) || float.IsNaN(maxt));
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
