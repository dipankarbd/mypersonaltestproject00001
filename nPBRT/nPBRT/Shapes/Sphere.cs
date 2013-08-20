using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nPBRT.Core;


namespace nPBRT.Shapes
{
    public class Sphere : Shape
    {
        private double radius;
        private double phiMax;
        private double zmin, zmax;
        private double thetaMin, thetaMax;

        public Sphere(Transform o2w, Transform w2o, bool ro, double rad, double z0, double z1, double pm)
            : base(o2w, w2o, ro)
        {
            radius = rad;
            zmin = Utility.Clamp(Math.Min(z0, z1), -radius, radius);
            zmax = Utility.Clamp(Math.Max(z0, z1), -radius, radius);
            thetaMin = Math.Acos(Utility.Clamp(zmin / radius, -1.0d, 1.0d));
            thetaMax = Math.Acos(Utility.Clamp(zmax / radius, -1.0d, 1.0d));
            phiMax = Utility.Radians(Utility.Clamp(pm, 0.0f, 360.0f));
        }

        public override BBox ObjectBound()
        {
            return new BBox(new Point(-radius, -radius, zmin),
                new Point(radius, radius, zmax));
        }

        public override bool Intersect(Ray r, out double tHit, out double rayEpsilon, out  DifferentialGeometry dg)
        {
            double phi;
            Point phit;

            tHit = Double.NaN;
            rayEpsilon = Double.NaN;
            dg = null;

            // Transform _Ray_ to object space
            Ray ray = WorldToObject.Apply(r);

            // Compute quadratic sphere coefficients
            double A = ray.d.x * ray.d.x + ray.d.y * ray.d.y + ray.d.z * ray.d.z;
            double B = 2 * (ray.d.x * ray.o.x + ray.d.y * ray.o.y + ray.d.z * ray.o.z);
            double C = ray.o.x * ray.o.x + ray.o.y * ray.o.y + ray.o.z * ray.o.z - radius * radius;

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

            // Compute sphere hit position and $\phi$
            phit = ray.GetPointAt(thit);
            if (phit.x == 0.0d && phit.y == 0.0d) phit.x = 1e-5d * radius;
            phi = Math.Atan2(phit.y, phit.x);
            if (phi < 0.0d) phi += 2.0d * Math.PI;

            // Test sphere intersection against clipping parameters
            if ((zmin > -radius && phit.z < zmin) ||
                (zmax < radius && phit.z > zmax) || phi > phiMax)
            {
                if (thit == t1) return false;
                if (t1 > ray.maxt) return false;
                thit = t1;
                // Compute sphere hit position and $\phi$
                phit = ray.GetPointAt(thit);
                if (phit.x == 0.0d && phit.y == 0.0d) phit.x = 1e-5d * radius;
                phi = Math.Atan2(phit.y, phit.x);
                if (phi < 0.0d) phi += 2.0d * Math.PI;
                if ((zmin > -radius && phit.z < zmin) ||
                    (zmax < radius && phit.z > zmax) || phi > phiMax)
                    return false;
            }

            // Find parametric representation of sphere hit
            double u = phi / phiMax;
            double theta = Math.Acos(Utility.Clamp(phit.z / radius, -1.0d, 1.0d));
            double v = (theta - thetaMin) / (thetaMax - thetaMin);

            // Compute sphere $\dpdu$ and $\dpdv$
            double zradius = Math.Sqrt(phit.x * phit.x + phit.y * phit.y);
            double invzradius = 1.0d / zradius;
            double cosphi = phit.x * invzradius;
            double sinphi = phit.y * invzradius;
            Vector dpdu = new Vector(-phiMax * phit.y, phiMax * phit.x, 0);
            Vector dpdv = (thetaMax - thetaMin) * new Vector(phit.z * cosphi, phit.z * sinphi, -radius * Math.Sin(theta));

            // Compute sphere $\dndu$ and $\dndv$
            Vector d2Pduu = -phiMax * phiMax * new Vector(phit.x, phit.y, 0);
            Vector d2Pduv = (thetaMax - thetaMin) * phit.z * phiMax * new Vector(-sinphi, cosphi, 0.0d);
            Vector d2Pdvv = -(thetaMax - thetaMin) * (thetaMax - thetaMin) * new Vector(phit.x, phit.y, phit.z);

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
            dg = new DifferentialGeometry(o2w.Apply(phit), o2w.Apply(dpdu), o2w.Apply(dpdv),
                                       o2w.Apply(dndu), o2w.Apply(dndv), u, v, this);

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

            // Compute quadratic sphere coefficients
            double A = ray.d.x * ray.d.x + ray.d.y * ray.d.y + ray.d.z * ray.d.z;
            double B = 2 * (ray.d.x * ray.o.x + ray.d.y * ray.o.y + ray.d.z * ray.o.z);
            double C = ray.o.x * ray.o.x + ray.o.y * ray.o.y + ray.o.z * ray.o.z - radius * radius;

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

            // Compute sphere hit position and $\phi$
            phit = ray.GetPointAt(thit);
            if (phit.x == 0.0d && phit.y == 0.0d) phit.x = 1e-5d * radius;
            phi = Math.Atan2(phit.y, phit.x);
            if (phi < 0.0d) phi += 2.0d * Math.PI;

            // Test sphere intersection against clipping parameters
            if ((zmin > -radius && phit.z < zmin) ||
                (zmax < radius && phit.z > zmax) || phi > phiMax)
            {
                if (thit == t1) return false;
                if (t1 > ray.maxt) return false;
                thit = t1;
                // Compute sphere hit position and $\phi$
                phit = ray.GetPointAt(thit);
                if (phit.x == 0.0d && phit.y == 0.0d) phit.x = 1e-5d * radius;
                phi = Math.Atan2(phit.y, phit.x);
                if (phi < 0.0d) phi += 2.0d * Math.PI;
                if ((zmin > -radius && phit.z < zmin) ||
                    (zmax < radius && phit.z > zmax) || phi > phiMax)
                    return false;
            }
            return true;
        }

        public override double Area()
        {
            return phiMax * radius * (zmax - zmin);
        }

        public override Point Sample(double u1, double u2, ref Normal ns)
        {
            Point p = new Point(0, 0, 0) + radius * MonteCarlo.UniformSampleSphere(u1, u2);
            ns = Geometry.Normalize(ObjectToWorld.Apply(new Normal(p.x, p.y, p.z)));
            if (ReverseOrientation) ns *= -1.0d;
            return ObjectToWorld.Apply(p);
        }

        public override Point Sample(Point p, double u1, double u2, ref Normal ns)
        {
            // Compute coordinate system for sphere sampling
            Point Pcenter = ObjectToWorld.Apply(new Point(0, 0, 0));
            Vector wc = Geometry.Normalize(Pcenter - p);
            Vector wcX, wcY;
            Geometry.CoordinateSystem(wc, out wcX, out wcY);

            // Sample uniformly on sphere if $\pt{}$ is inside it
            if (Geometry.DistanceSquared(p, Pcenter) - radius * radius < 1e-4f)
                return Sample(u1, u2, ref ns);

            // Sample sphere uniformly inside subtended cone
            double sinThetaMax2 = radius * radius / Geometry.DistanceSquared(p, Pcenter);
            double cosThetaMax = Math.Sqrt(Math.Max(0.0d, 1.0d - sinThetaMax2));
            DifferentialGeometry dgSphere;
            double thit, rayEpsilon;
            Point ps;
            Ray r = new Ray(p, MonteCarlo.UniformSampleCone(u1, u2, cosThetaMax, wcX, wcY, wc), 1e-3d);
            if (!Intersect(r, out thit, out rayEpsilon, out dgSphere))
                thit = Geometry.Dot(Pcenter - p, Geometry.Normalize(r.d));
            ps = r.GetPointAt(thit);
            ns = new Normal(Geometry.Normalize(ps - Pcenter));
            if (ReverseOrientation) ns *= -1.0d;
            return ps;
        }

        public override double Pdf(Point p, Vector wi)
        {
            Point Pcenter = ObjectToWorld.Apply(new Point(0, 0, 0));
            // Return uniform weight if point inside sphere
            if (Geometry.DistanceSquared(p, Pcenter) - radius * radius < 1e-4d)
                return base.Pdf(p, wi);

            // Compute general sphere weight
            double sinThetaMax2 = radius * radius / Geometry.DistanceSquared(p, Pcenter);
            double cosThetaMax = Math.Sqrt(Math.Max(0.0d, 1.0d - sinThetaMax2));
            return MonteCarlo.UniformConePdf(cosThetaMax);
        }

        public static Sphere CreateSphereShape(Transform o2w, Transform w2o, bool reverseOrientation, ParamSet parameters)
        {
            double radius = parameters.FindOneDouble("radius", 1.0d);
            double zmin = parameters.FindOneDouble("zmin", -radius);
            double zmax = parameters.FindOneDouble("zmax", radius);
            double phimax = parameters.FindOneDouble("phimax", 360.0d);
            return new Sphere(o2w, w2o, reverseOrientation, radius, zmin, zmax, phimax);
        }
    }
}
