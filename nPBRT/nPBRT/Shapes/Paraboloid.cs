﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nPBRT.Core;

namespace nPBRT.Shapes
{
    public class Paraboloid : Shape
    {
        protected double radius;
        protected double zmin, zmax;
        protected double phiMax;

        public Paraboloid(Transform o2w, Transform w2o, bool ro, double rad, double z0, double z1, double tm)
            : base(o2w, w2o, ro)
        {
            radius = rad;
            zmin = Math.Min(z0, z1);
            zmax = Math.Max(z0, z1);
            phiMax = Utility.Radians(Utility.Clamp(tm, 0.0d, 360.0d));
        }

        public override BBox ObjectBound()
        {
            Point p1 = new Point(-radius, -radius, zmin);
            Point p2 = new Point(radius, radius, zmax);
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

            // Compute quadratic paraboloid coefficients
            double k = zmax / (radius * radius);
            double A = k * (ray.d.x * ray.d.x + ray.d.y * ray.d.y);
            double B = 2 * k * (ray.d.x * ray.o.x + ray.d.y * ray.o.y) -
                        ray.d.z;
            double C = k * (ray.o.x * ray.o.x + ray.o.y * ray.o.y) -
                        ray.o.z;

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

            // Compute paraboloid inverse mapping
            phit = ray.GetPointAt(thit);
            phi = Math.Atan2(phit.y, phit.x);
            if (phi < 0.0d) phi += 2.0d * Math.PI;

            // Test paraboloid intersection against clipping parameters
            if (phit.z < zmin || phit.z > zmax || phi > phiMax)
            {
                if (thit == t1) return false;
                thit = t1;
                if (t1 > ray.maxt) return false;
                // Compute paraboloid inverse mapping
                phit = ray.GetPointAt(thit);
                phi = Math.Atan2(phit.y, phit.x);
                if (phi < 0.0d) phi += 2.0d * Math.PI;
                if (phit.z < zmin || phit.z > zmax || phi > phiMax)
                    return false;
            }

            // Find parametric representation of paraboloid hit
            double u = phi / phiMax;
            double v = (phit.z - zmin) / (zmax - zmin);

            // Compute parabaloid $\dpdu$ and $\dpdv$
            Vector dpdu = new Vector(-phiMax * phit.y, phiMax * phit.x, 0.0d);
            Vector dpdv = (zmax - zmin) * new Vector(phit.x / (2.0d * phit.z), phit.y / (2.0d * phit.z), 1.0d);

            // Compute parabaloid $\dndu$ and $\dndv$
            Vector d2Pduu = -phiMax * phiMax * new Vector(phit.x, phit.y, 0);
            Vector d2Pduv = (zmax - zmin) * phiMax * new Vector(-phit.y / (2.0d * phit.z), phit.x / (2.0d * phit.z), 0);
            Vector d2Pdvv = -(zmax - zmin) * (zmax - zmin) * new Vector(phit.x / (4.0d * phit.z * phit.z), phit.y / (4.0d * phit.z * phit.z), 0.0d);

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

            // Compute quadratic paraboloid coefficients
            double k = zmax / (radius * radius);
            double A = k * (ray.d.x * ray.d.x + ray.d.y * ray.d.y);
            double B = 2 * k * (ray.d.x * ray.o.x + ray.d.y * ray.o.y) - ray.d.z;
            double C = k * (ray.o.x * ray.o.x + ray.o.y * ray.o.y) - ray.o.z;

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

            // Compute paraboloid inverse mapping
            phit = ray.GetPointAt(thit);
            phi = Math.Atan2(phit.y, phit.x);
            if (phi < 0.0d) phi += 2.0d * Math.PI;

            // Test paraboloid intersection against clipping parameters
            if (phit.z < zmin || phit.z > zmax || phi > phiMax)
            {
                if (thit == t1) return false;
                thit = t1;
                if (t1 > ray.maxt) return false;
                // Compute paraboloid inverse mapping
                phit = ray.GetPointAt(thit);
                phi = Math.Atan2(phit.y, phit.x);
                if (phi < 0.0d) phi += 2.0d * Math.PI;
                if (phit.z < zmin || phit.z > zmax || phi > phiMax)
                    return false;
            }
            return true;
        }

        public override double Area()
        {
            return phiMax / 12.0d * (Math.Pow(1 + 4 * zmin, 1.5d) - Math.Pow(1 + 4 * zmax, 1.5d));
        }

        public static Paraboloid CreateParaboloidShape(Transform o2w, Transform w2o, bool reverseOrientation, ParamSet parameters)
        {
            double radius = parameters.FindOneDouble("radius", 1);
            double zmin = parameters.FindOneDouble("zmin", 0);
            double zmax = parameters.FindOneDouble("zmax", 1);
            double phimax = parameters.FindOneDouble("phimax", 360);
            return new Paraboloid(o2w, w2o, reverseOrientation, radius, zmin, zmax, phimax);
        }

    }
}
