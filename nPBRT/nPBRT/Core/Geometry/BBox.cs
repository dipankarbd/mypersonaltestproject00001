using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core.Geometry
{
    public class BBox
    {
        public Point pMin, pMax;
        public BBox()
        {
            pMin = new Point(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            pMax = new Point(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
        }
        public BBox(Point p)
        {
            pMin = new Point(p.x, p.y, p.z);
            pMax = new Point(p.x, p.y, p.z);
        }
        public BBox(Point p1, Point p2)
        {
            pMin = new Point(Math.Min(p1.x, p2.x), Math.Min(p1.y, p2.y), Math.Min(p1.z, p2.z));
            pMax = new Point(Math.Max(p1.x, p2.x), Math.Max(p1.y, p2.y), Math.Max(p1.z, p2.z));
        }
        public bool Overlaps(BBox b)
        {
            bool x = (pMax.x >= b.pMin.x) && (pMin.x <= b.pMax.x);
            bool y = (pMax.y >= b.pMin.y) && (pMin.y <= b.pMax.y);
            bool z = (pMax.z >= b.pMin.z) && (pMin.z <= b.pMax.z);
            return (x && y && z);
        }
        public bool Inside(Point pt)
        {
            return (pt.x >= pMin.x && pt.x <= pMax.x &&
                    pt.y >= pMin.y && pt.y <= pMax.y &&
                    pt.z >= pMin.z && pt.z <= pMax.z);
        }
        public void Expand(double delta)
        {
            pMin -= new Vector(delta, delta, delta);
            pMax += new Vector(delta, delta, delta);
        }
        public double SurfaceArea()
        {
            Vector d = pMax - pMin;
            return 2.0d * (d.x * d.y + d.x * d.z + d.y * d.z);
        }
        public double Volume()
        {
            Vector d = pMax - pMin;
            return d.x * d.y * d.z;
        }
        public int MaximumExtent()
        {
            Vector diag = pMax - pMin;
            if (diag.x > diag.y && diag.x > diag.z)
                return 0;
            else if (diag.y > diag.z)
                return 1;
            else
                return 2;
        }
        public Point Lerp(double tx, double ty, double tz)
        {
            return new Point(Utility.Lerp(tx, pMin.x, pMax.x),
                Utility.Lerp(ty, pMin.y, pMax.y),
                Utility.Lerp(tz, pMin.z, pMax.z));
        }
        public Vector Offset(Point p)
        {
            return new Vector((p.x - pMin.x) / (pMax.x - pMin.x),
                          (p.y - pMin.y) / (pMax.y - pMin.y),
                          (p.z - pMin.z) / (pMax.z - pMin.z));
        }
        public void BoundingSphere(out Point c, out double rad)
        {
            c = (0.5f * pMin) + (0.5f * pMax);
            rad = Inside(c) ? Geometry.Distance(c, pMax) : 0.0d;
        }
        public bool IntersectP(Ray ray, out double hitt0, out double hitt1)
        {
            hitt0 = hitt1 = double.PositiveInfinity;
            double t0 = ray.mint, t1 = ray.maxt;
            for (int i = 0; i < 3; ++i)
            {
                // Update interval for _i_th bounding box slab
                double invRayDir = 1.0d / ray.d[i];
                double tNear = (pMin[i] - ray.o[i]) * invRayDir;
                double tFar = (pMax[i] - ray.o[i]) * invRayDir;

                // Update parametric interval from slab intersection $t$s
                if (tNear > tFar) Utility.Swap<double>(ref tNear, ref  tFar);
                t0 = tNear > t0 ? tNear : t0;
                t1 = tFar < t1 ? tFar : t1;
                if (t0 > t1) return false;
            }
            hitt0 = t0;
            hitt1 = t1;
            return true;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            BBox b = obj as BBox;
            if ((System.Object)b == null)
            {
                return false;
            }

            return b.pMin == pMin && b.pMax == pMax;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public Point this[int index]
        {
            get
            {
                if (index == 0 || index ==1) throw new ArgumentException();

                if (index == 0) return pMin;
                else if (index == 1) return pMax;
                else return null;
            }
        }
        public BBox Copy()
        {
            return new BBox(this.pMin, this.pMax); 
        }
    }
}
