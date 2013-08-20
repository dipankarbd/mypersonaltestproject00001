using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nPBRT.Core;

namespace nPBRT.Shapes
{
    public class Cone : Shape
    {
        protected double radius, height, phiMax;

        public Cone(Transform o2w, Transform w2o, bool ro, double ht, double rad, double tm)
            : base(o2w, w2o, ro)
        {
            radius = rad;
            height = ht;
            phiMax = Utility.Radians(Utility.Clamp(tm, 0.0d, 360.0d));
        }

        public override BBox ObjectBound()
        {
            Point p1 = new Point(-radius, -radius, 0);
            Point p2 = new Point(radius, radius, height);
            return new BBox(p1, p2);
        }

        public override bool Intersect(Ray r, out double tHit, out double rayEpsilon, out DifferentialGeometry dg)
        {
            double phi;
            Point phit;

            tHit = Double.NaN;
            rayEpsilon = Double.NaN;
            dg = null;

            // Transform _Ray_ to object space
            Ray ray = WorldToObject.Apply(r);

            // Compute quadratic cone coefficients
            double k = radius / height;
            k = k * k;
            double A = ray.d.x * ray.d.x + ray.d.y * ray.d.y - k * ray.d.z * ray.d.z;
            double B = 2 * (ray.d.x * ray.o.x + ray.d.y * ray.o.y - k * ray.d.z * (ray.o.z - height));
            double C = ray.o.x * ray.o.x + ray.o.y * ray.o.y - k * (ray.o.z - height) * (ray.o.z - height);

            // Solve quadratic equation for _t_ values
            double t0, t1;
            if (!Utility.Quadratic(A, B, C, out t0, out t1))
                return false;

            // Compute intersection distance along ray
            if (t0 > ray.maxt || t1 < ray.mint)
                return false;
            double thit = t0;
            if (t0 < ray.mint)
            {
                thit = t1;
                if (thit > ray.maxt) return false;
            }

            // Compute cone inverse mapping
            phit = ray.GetPointAt(thit);
            phi = Math.Atan2(phit.y, phit.x);
            if (phi < 0.0d) phi += 2.0d * Math.PI;

            // Test cone intersection against clipping parameters
            if (phit.z < 0 || phit.z > height || phi > phiMax)
            {
                if (thit == t1) return false;
                thit = t1;
                if (t1 > ray.maxt) return false;
                // Compute cone inverse mapping
                phit = ray.GetPointAt(thit);
                phi = Math.Atan2(phit.y, phit.x);
                if (phi < 0.0d) phi += 2.0d * Math.PI;
                if (phit.z < 0 || phit.z > height || phi > phiMax)
                    return false;
            }

            // Find parametric representation of cone hit
            double u = phi / phiMax;
            double v = phit.z / height;

            // Compute cone $\dpdu$ and $\dpdv$
            Vector dpdu = new Vector(-phiMax * phit.y, phiMax * phit.x, 0);
            Vector dpdv = new Vector(-phit.x / (1.0d - v), -phit.y / (1.0d - v), height);

            // Compute cone $\dndu$ and $\dndv$
            Vector d2Pduu = -phiMax * phiMax * new Vector(phit.x, phit.y, 0.0d);
            Vector d2Pduv = phiMax / (1.0d - v) * new Vector(phit.y, -phit.x, 0.0d);
            Vector d2Pdvv = new Vector(0, 0, 0);

            // Compute coefficients for fundamental forms
            double E = Geometry.Dot(dpdu, dpdu);
            double F = Geometry.Dot(dpdu, dpdv);
            double G = Geometry.Dot(dpdv, dpdv);
            Vector N = Geometry.Normalize(Geometry.Cross(dpdu, dpdv));
            double e = Geometry.Dot(N, d2Pduu);
            double f = Geometry.Dot(N, d2Pduv);
            double g = Geometry.Dot(N, d2Pdvv);

            // Compute $\dndu$ and $\dndv$ from fundamental form coefficients
            double invEGF2 = 1.0d / (E * G - F * F);
            Normal dndu = new Normal((f * F - e * G) * invEGF2 * dpdu + (e * F - f * E) * invEGF2 * dpdv);
            Normal dndv = new Normal((g * F - f * G) * invEGF2 * dpdu + (f * F - g * E) * invEGF2 * dpdv);

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
            double phi;
            Point phit;
            // Transform _Ray_ to object space
            Ray ray = WorldToObject.Apply(r);

            // Compute quadratic cone coefficients
            double k = radius / height;
            k = k * k;
            double A = ray.d.x * ray.d.x + ray.d.y * ray.d.y -
                k * ray.d.z * ray.d.z;
            double B = 2 * (ray.d.x * ray.o.x + ray.d.y * ray.o.y -
                k * ray.d.z * (ray.o.z - height));
            double C = ray.o.x * ray.o.x + ray.o.y * ray.o.y -
                k * (ray.o.z - height) * (ray.o.z - height);

            // Solve quadratic equation for _t_ values
            double t0, t1;
            if (!Utility.Quadratic(A, B, C, out t0, out t1))
                return false;

            // Compute intersection distance along ray
            if (t0 > ray.maxt || t1 < ray.mint)
                return false;
            double thit = t0;
            if (t0 < ray.mint)
            {
                thit = t1;
                if (thit > ray.maxt) return false;
            }

            // Compute cone inverse mapping
            phit = ray.GetPointAt(thit);
            phi = Math.Atan2(phit.y, phit.x);
            if (phi < 0.0d) phi += 2.0d * Math.PI;

            // Test cone intersection against clipping parameters
            if (phit.z < 0 || phit.z > height || phi > phiMax)
            {
                if (thit == t1) return false;
                thit = t1;
                if (t1 > ray.maxt) return false;
                // Compute cone inverse mapping
                phit = ray.GetPointAt(thit);
                phi = Math.Atan2(phit.y, phit.x);
                if (phi < 0.0d) phi += 2.0d * Math.PI;
                if (phit.z < 0 || phit.z > height || phi > phiMax)
                    return false;
            }
            return true;
        }

        public override double Area()
        {
            return radius * Math.Sqrt((height * height) + (radius * radius)) * phiMax / 2.0d;
        }
         
        public static Cone CreateConeShape(Transform o2w, Transform w2o, bool reverseOrientation, ParamSet parameters)
        {
            double radius = parameters.FindOneDouble("radius", 1);
            double height = parameters.FindOneDouble("height", 1);
            double phimax = parameters.FindOneDouble("phimax", 360);
            return new Cone(o2w, w2o, reverseOrientation, height, radius, phimax);
        }

    }
}
