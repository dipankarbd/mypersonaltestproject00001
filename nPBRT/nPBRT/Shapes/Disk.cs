using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nPBRT.Core;

namespace nPBRT.Shapes
{
    public class Disk : Shape
    {
        private double height, radius, innerRadius, phiMax;

        public Disk(Transform o2w, Transform w2o, bool ro, double ht, double r, double ri, double tmax)
            : base(o2w, w2o, ro)
        {
            height = ht;
            radius = r;
            innerRadius = ri;
            phiMax = Utility.Radians(Utility.Clamp(tmax, 0.0d, 360.0d));
        }

        public override BBox ObjectBound()
        {
            return new BBox(new Point(-radius, -radius, height), new Point(radius, radius, height));
        }

        public override bool Intersect(Ray r, out double tHit, out double rayEpsilon, out DifferentialGeometry dg)
        {
            tHit = Double.NaN;
            rayEpsilon = Double.NaN;
            dg = null;

            // Transform _Ray_ to object space
            Ray ray = WorldToObject.Apply(r);

            // Compute plane intersection for disk
            if (Math.Abs(ray.d.z) < 1e-7) return false;
            double thit = (height - ray.o.z) / ray.d.z;
            if (thit < ray.mint || thit > ray.maxt)
                return false;

            // See if hit point is inside disk radii and $\phimax$
            Point phit = ray.GetPointAt(thit);
            double dist2 = phit.x * phit.x + phit.y * phit.y;
            if (dist2 > radius * radius || dist2 < innerRadius * innerRadius)
                return false;

            // Test disk $\phi$ value against $\phimax$
            double phi = Math.Atan2(phit.y, phit.x);
            if (phi < 0) phi += 2.0d * Math.PI;
            if (phi > phiMax)
                return false;

            // Find parametric representation of disk hit
            double u = phi / phiMax;
            double oneMinusV = ((Math.Sqrt(dist2) - innerRadius) / (radius - innerRadius));
            double invOneMinusV = (oneMinusV > 0.0d) ? (1.0d / oneMinusV) : 0.0d;
            double v = 1.0d - oneMinusV;
            Vector dpdu = new Vector(-phiMax * phit.y, phiMax * phit.x, 0.0d);
            Vector dpdv = new Vector(-phit.x * invOneMinusV, -phit.y * invOneMinusV, 0.0d);
            dpdu *= phiMax * Constants.INV_TWOPI;
            dpdv *= (radius - innerRadius) / radius;
            Normal dndu = new Normal(0, 0, 0);
            Normal dndv = new Normal(0, 0, 0);

            // Initialize _DifferentialGeometry_ from parametric information
            Transform o2w = ObjectToWorld;
            dg = new DifferentialGeometry(o2w.Apply(phit), o2w.Apply(dpdu), o2w.Apply(dpdv), o2w.Apply(dndu), o2w.Apply(dndv), u, v, this);

            // Update _tHit_ for quadric intersection
            tHit = thit;

            // Compute _rayEpsilon_ for quadric intersection
            rayEpsilon = 5e-4d * tHit;
            return true;
        }

        public override bool IntersectP(Ray r)
        {
            // Transform _Ray_ to object space
            Ray ray = WorldToObject.Apply(r);

            // Compute plane intersection for disk
            if (Math.Abs(ray.d.z) < 1e-7) return false;
            double thit = (height - ray.o.z) / ray.d.z;
            if (thit < ray.mint || thit > ray.maxt)
                return false;

            // See if hit point is inside disk radii and $\phimax$
            Point phit = ray.GetPointAt(thit);
            double dist2 = phit.x * phit.x + phit.y * phit.y;
            if (dist2 > radius * radius || dist2 < innerRadius * innerRadius)
                return false;

            // Test disk $\phi$ value against $\phimax$
            double phi = Math.Atan2(phit.y, phit.x);
            if (phi < 0) phi += 2.0d * Math.PI;
            if (phi > phiMax)
                return false;
            return true;
        }

        public override double Area()
        {
            return phiMax * 0.5d * (radius * radius - innerRadius * innerRadius);
        }

        public override Point Sample(double u1, double u2, ref Normal ns)
        {
            Point p = new Point();
            MonteCarlo.ConcentricSampleDisk(u1, u2, ref p.x, ref p.y);
            p.x *= radius;
            p.y *= radius;
            p.z = height;
            ns = Geometry.Normalize(ObjectToWorld.Apply(new Normal(0, 0, 1)));
            if (ReverseOrientation) ns *= -1.0d;
            return ObjectToWorld.Apply(p);
        }

        public static Disk CreateDiskShape(Transform o2w, Transform w2o, bool reverseOrientation, ParamSet parameters)
        {
            double height = parameters.FindOneDouble("height", 0.0d);
            double radius = parameters.FindOneDouble("radius", 1);
            double inner_radius = parameters.FindOneDouble("innerradius", 0);
            double phimax = parameters.FindOneDouble("phimax", 360);
            return new Disk(o2w, w2o, reverseOrientation, height, radius, inner_radius, phimax);
        }
    }
}
