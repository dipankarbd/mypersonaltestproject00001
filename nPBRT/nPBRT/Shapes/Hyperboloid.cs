using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nPBRT.Core;

namespace nPBRT.Shapes
{
    public class Hyperboloid : Shape
    {
        protected Point p1, p2;
        protected double zmin, zmax;
        protected double phiMax;
        protected double rmax;
        protected double a, c;

        public Hyperboloid(Transform o2w, Transform w2o, bool ro,
         Point point1, Point point2, double tm)
            : base(o2w, w2o, ro)
        {
            p1 = point1;
            p2 = point2;
            phiMax = Utility.Radians(Utility.Clamp(tm, 0.0d, 360.0d));
            double radius1 = Math.Sqrt(p1.x * p1.x + p1.y * p1.y);
            double radius2 = Math.Sqrt(p2.x * p2.x + p2.y * p2.y);
            rmax = Math.Max(radius1, radius2);
            zmin = Math.Min(p1.z, p2.z);
            zmax = Math.Max(p1.z, p2.z);

            // Compute implicit function coefficients for hyperboloid
            if (p2.z == 0.0d) Utility.Swap<Point>(ref p1, ref  p2);
            Point pp = p1;
            double xy1, xy2;
            do
            {
                pp += 2.0d * (p2 - p1);
                xy1 = pp.x * pp.x + pp.y * pp.y;
                xy2 = p2.x * p2.x + p2.y * p2.y;
                a = (1.0d / xy1 - (pp.z * pp.z) / (xy1 * p2.z * p2.z)) /
                    (1 - (xy2 * pp.z * pp.z) / (xy1 * p2.z * p2.z));
                c = (a * xy2 - 1) / (p2.z * p2.z);
            } while (Double.IsInfinity(a) || Double.IsNaN(a));
        }
        public override BBox ObjectBound()
        {
            Point p1 = new Point(-rmax, -rmax, zmin);
            Point p2 = new Point(rmax, rmax, zmax);
            return new BBox(p1, p2);
        }

        public override bool Intersect(Ray r, out double tHit, out double rayEpsilon, out DifferentialGeometry dg)
        {
            double phi, v;
            Point phit;


            tHit = Double.NaN;
            rayEpsilon = Double.NaN;
            dg = null;


            // Transform _Ray_ to object space
            Ray ray = WorldToObject.Apply(r);

            // Compute quadratic hyperboloid coefficients
            double A = a * ray.d.x * ray.d.x + a * ray.d.y * ray.d.y - c * ray.d.z * ray.d.z;
            double B = 2.0d * (a * ray.d.x * ray.o.x + a * ray.d.y * ray.o.y - c * ray.d.z * ray.o.z);
            double C = a * ray.o.x * ray.o.x +
                      a * ray.o.y * ray.o.y -
                      c * ray.o.z * ray.o.z - 1;

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

            // Compute hyperboloid inverse mapping
            phit = ray.GetPointAt(thit);
            v = (phit.z - p1.z) / (p2.z - p1.z);
            Point pr = (1.0d - v) * p1 + v * p2;
            phi = Math.Atan2(pr.x * phit.y - phit.x * pr.y,
                phit.x * pr.x + phit.y * pr.y);
            if (phi < 0)
                phi += 2 * Math.PI;

            // Test hyperboloid intersection against clipping parameters
            if (phit.z < zmin || phit.z > zmax || phi > phiMax)
            {
                if (thit == t1) return false;
                thit = t1;
                if (t1 > ray.maxt) return false;
                // Compute hyperboloid inverse mapping
                phit = ray.GetPointAt(thit);
                v = (phit.z - p1.z) / (p2.z - p1.z);
                pr = (1.0d - v) * p1 + v * p2;
                phi = Math.Atan2(pr.x * phit.y - phit.x * pr.y,
                    phit.x * pr.x + phit.y * pr.y);
                if (phi < 0)
                    phi += 2 * Math.PI;
                if (phit.z < zmin || phit.z > zmax || phi > phiMax)
                    return false;
            }

            // Compute parametric representation of hyperboloid hit
            double u = phi / phiMax;

            // Compute hyperboloid $\dpdu$ and $\dpdv$
            double cosphi = Math.Cos(phi), sinphi = Math.Sin(phi);
            Vector dpdu = new Vector(-phiMax * phit.y, phiMax * phit.x, 0.0d);
            Vector dpdv = new Vector((p2.x - p1.x) * cosphi - (p2.y - p1.y) * sinphi, (p2.x - p1.x) * sinphi + (p2.y - p1.y) * cosphi, p2.z - p1.z);

            // Compute hyperboloid $\dndu$ and $\dndv$
            Vector d2Pduu = -phiMax * phiMax * new Vector(phit.x, phit.y, 0);
            Vector d2Pduv = phiMax * new Vector(-dpdv.y, dpdv.x, 0.0d);
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
            double phi, v;
            Point phit;

            // Transform _Ray_ to object space
            Ray ray = WorldToObject.Apply(r);

            // Compute quadratic hyperboloid coefficients
            double A = a * ray.d.x * ray.d.x + a * ray.d.y * ray.d.y - c * ray.d.z * ray.d.z;
            double B = 2.0d * (a * ray.d.x * ray.o.x + a * ray.d.y * ray.o.y - c * ray.d.z * ray.o.z);
            double C = a * ray.o.x * ray.o.x + a * ray.o.y * ray.o.y - c * ray.o.z * ray.o.z - 1;

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

            // Compute hyperboloid inverse mapping
            phit = ray.GetPointAt(thit);
            v = (phit.z - p1.z) / (p2.z - p1.z);
            Point pr = (1.0d - v) * p1 + v * p2;
            phi = Math.Atan2(pr.x * phit.y - phit.x * pr.y,
                phit.x * pr.x + phit.y * pr.y);
            if (phi < 0)
                phi += 2 * Math.PI;

            // Test hyperboloid intersection against clipping parameters
            if (phit.z < zmin || phit.z > zmax || phi > phiMax)
            {
                if (thit == t1) return false;
                thit = t1;
                if (t1 > ray.maxt) return false;
                // Compute hyperboloid inverse mapping
                phit = ray.GetPointAt(thit);
                v = (phit.z - p1.z) / (p2.z - p1.z);
                pr = (1.0d - v) * p1 + v * p2;
                phi = Math.Atan2(pr.x * phit.y - phit.x * pr.y,
                    phit.x * pr.x + phit.y * pr.y);
                if (phi < 0)
                    phi += 2 * Math.PI;
                if (phit.z < zmin || phit.z > zmax || phi > phiMax)
                    return false;
            }
            return true;
        }

        public override double Area()
        {
            return phiMax / 6.0d * (2.0d * Math.Pow(p1.x, 4) - 2.0d * p1.x * p1.x * p1.x * p2.x
                + 2.0d * Math.Pow(p2.x, 4) + 2.0d * (p1.y * p1.y + p1.y * p2.y + p2.y * p2.y) * (Math.Pow(p1.y - p2.y, 2) + Math.Pow(p1.z - p2.z, 2))
                + p2.x * p2.x * (5.0d * p1.y * p1.y + 2.0d * p1.y * p2.y - 4.0d * p2.y * p2.y + 2.0d * Math.Pow(p1.z - p2.z, 2))
                + p1.x * p1.x * (-4.0d * p1.y * p1.y + 2.0d * p1.y * p2.y + 5.0d * p2.y * p2.y + 2.0d * Math.Pow(p1.z - p2.z, 2))
                - 2.0d * p1.x * p2.x * (p2.x * p2.x - p1.y * p1.y + 5.0d * p1.y * p2.y - p2.y * p2.y - p1.z * p1.z
                + 2.0d * p1.z * p2.z - p2.z * p2.z));
        }

        public static Hyperboloid CreateHyperboloidShape(Transform o2w, Transform w2o, bool reverseOrientation, ParamSet parameters)
        {
            Point p1 = parameters.FindOnePoint("p1", new Point(0, 0, 0));
            Point p2 = parameters.FindOnePoint("p2", new Point(1, 1, 1));
            double phimax = parameters.FindOneDouble("phimax", 360);
            return new Hyperboloid(o2w, w2o, reverseOrientation, p1, p2, phimax);
        }
    }
}
