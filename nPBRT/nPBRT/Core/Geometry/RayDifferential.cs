using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core.Geometry
{
    public class RayDifferential : Ray
    {
        public bool hasDifferentials;
        public Point rxOrigin, ryOrigin;
        public Vector rxDirection, ryDirection;

        public RayDifferential()
        {
            this.hasDifferentials = false;
        }
        public RayDifferential(Point org, Vector dir, float start, float end = float.PositiveInfinity, float t = 0.0f, int d = 0)
            : base(org, dir, start, end, t, d)
        {
            this.hasDifferentials = false;
        }
        public RayDifferential(Point org, Vector dir, Ray parent, float start, float end = float.PositiveInfinity)
            : base(org, dir, start, end, parent.time, parent.depth + 1)
        {
            this.hasDifferentials = false;
        }
        public RayDifferential(Ray ray)
            : base(ray.o, ray.d, ray.mint, ray.maxt, ray.time, ray.depth)
        {
            this.hasDifferentials = false;
        }
        public override bool HasNaNs()
        {
            return base.HasNaNs() || (hasDifferentials && (rxOrigin.HasNaNs() || ryOrigin.HasNaNs() || rxDirection.HasNaNs() || ryDirection.HasNaNs()));
        }
        public void ScaleDifferentials(float s)
        {
            this.rxOrigin = o + (rxOrigin - o) * s;
            this.ryOrigin = o + (ryOrigin - o) * s;
            this.rxDirection = d + (rxDirection - d) * s;
            this.ryDirection = d + (ryDirection - d) * s;
        }
    }
}
