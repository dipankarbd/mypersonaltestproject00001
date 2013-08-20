using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nPBRT.Core;

namespace nPBRT.Shapes
{
    public class Triangle : Shape
    {
        private TriangleMesh mesh;
        private int[] v;

        public Triangle(Transform o2w, Transform w2o, bool ro, TriangleMesh m, int n)
            : base(o2w, w2o, ro)
        {
            mesh = m;
            v = new int[3];
            for (int i = 0; i < 3; i++)
            {
                v[i] = mesh.vertexIndex[3 * n + i];
            }
        }

        private void GetUVs(double[,] uv)
        {
            if (mesh.uvs != null)
            {
                uv[0, 0] = mesh.uvs[2 * v[0]];
                uv[0, 1] = mesh.uvs[2 * v[0] + 1];
                uv[1, 0] = mesh.uvs[2 * v[1]];
                uv[1, 1] = mesh.uvs[2 * v[1] + 1];
                uv[2, 0] = mesh.uvs[2 * v[2]];
                uv[2, 1] = mesh.uvs[2 * v[2] + 1];
            }
            else
            {
                uv[0, 0] = 0.0d; uv[0, 1] = 0.0d;
                uv[1, 0] = 1.0d; uv[1, 1] = 0.0d;
                uv[2, 0] = 1.0d; uv[2, 1] = 1.0d;
            }
        }

        public override BBox ObjectBound()
        {
            Point p1 = mesh.p[v[0]];
            Point p2 = mesh.p[v[1]];
            Point p3 = mesh.p[v[2]];
            return Geometry.Union(new BBox(WorldToObject.Apply(p1), WorldToObject.Apply(p2)), WorldToObject.Apply(p3));
        }

        public override BBox WorldBound()
        {
            Point p1 = mesh.p[v[0]];
            Point p2 = mesh.p[v[1]];
            Point p3 = mesh.p[v[2]];
            return Geometry.Union(new BBox(p1, p2), p3);
        }

        public override bool Intersect(Ray r, out double tHit, out double rayEpsilon, out DifferentialGeometry dg)
        {
            tHit = Double.NaN;
            rayEpsilon = Double.NaN;
            dg = null;

            // Compute $\VEC{s}_1$

            // Get triangle vertices in _p1_, _p2_, and _p3_
            Point p1 = mesh.p[v[0]];
            Point p2 = mesh.p[v[1]];
            Point p3 = mesh.p[v[2]];
            Vector e1 = p2 - p1;
            Vector e2 = p3 - p1;
            Vector s1 = Geometry.Cross(r.d, e2);
            double divisor = Geometry.Dot(s1, e1);

            if (divisor == 0.0d)
                return false;
            double invDivisor = 1.0d / divisor;

            // Compute first barycentric coordinate
            Vector s = r.o - p1;
            double b1 = Geometry.Dot(s, s1) * invDivisor;
            if (b1 < 0.0d || b1 > 1.0d)
                return false;

            // Compute second barycentric coordinate
            Vector s2 = Geometry.Cross(s, e1);
            double b2 = Geometry.Dot(r.d, s2) * invDivisor;
            if (b2 < 0.0d || b1 + b2 > 1.0d)
                return false;

            // Compute _t_ to intersection point
            double t = Geometry.Dot(e2, s2) * invDivisor;
            if (t < r.mint || t > r.maxt)
                return false;

            // Compute triangle partial derivatives
            Vector dpdu, dpdv;
            double[,] uvs = new double[3, 2];
            GetUVs(uvs);

            // Compute deltas for triangle partial derivatives
            double du1 = uvs[0, 0] - uvs[2, 0];
            double du2 = uvs[1, 0] - uvs[2, 0];
            double dv1 = uvs[0, 1] - uvs[2, 1];
            double dv2 = uvs[1, 1] - uvs[2, 1];
            Vector dp1 = p1 - p3, dp2 = p2 - p3;
            double determinant = du1 * dv2 - dv1 * du2;
            if (determinant == 0.0d)
            {
                // Handle zero determinant for triangle partial derivative matrix
                Geometry.CoordinateSystem(Geometry.Normalize(Geometry.Cross(e2, e1)), out dpdu, out dpdv);
            }
            else
            {
                double invdet = 1.0d / determinant;
                dpdu = (dv2 * dp1 - dv1 * dp2) * invdet;
                dpdv = (-du2 * dp1 + du1 * dp2) * invdet;
            }

            // Interpolate $(u,v)$ triangle parametric coordinates
            double b0 = 1 - b1 - b2;
            double tu = b0 * uvs[0, 0] + b1 * uvs[1, 0] + b2 * uvs[2, 0];
            double tv = b0 * uvs[0, 1] + b1 * uvs[1, 1] + b2 * uvs[2, 1];

            // Test intersection against alpha texture, if present
            if (r.depth != -1)
            {
                if (mesh.alphaTexture != null)
                {
                    DifferentialGeometry dgLocal = new DifferentialGeometry(r.GetPointAt(t), dpdu, dpdv, new Normal(0, 0, 0), new Normal(0, 0, 0), tu, tv, this);
                    if (mesh.alphaTexture.Evaluate(dgLocal) == 0.0d)
                        return false;
                }
            }

            // Fill in _DifferentialGeometry_ from triangle hit
            dg = new DifferentialGeometry(r.GetPointAt(t), dpdu, dpdv, new Normal(0, 0, 0), new Normal(0, 0, 0), tu, tv, this);
            tHit = t;
            rayEpsilon = 1e-3d * tHit;
            return true;
        }

        public override bool IntersectP(Ray r)
        {
            // Compute $\VEC{s}_1$

            // Get triangle vertices in _p1_, _p2_, and _p3_
            Point p1 = mesh.p[v[0]];
            Point p2 = mesh.p[v[1]];
            Point p3 = mesh.p[v[2]];
            Vector e1 = p2 - p1;
            Vector e2 = p3 - p1;
            Vector s1 = Geometry.Cross(r.d, e2);
            double divisor = Geometry.Dot(s1, e1);

            if (divisor == 0.0d)
                return false;
            double invDivisor = 1.0d / divisor;

            // Compute first barycentric coordinate
            Vector d = r.o - p1;
            double b1 = Geometry.Dot(d, s1) * invDivisor;
            if (b1 < 0.0d || b1 > 1.0d)
                return false;

            // Compute second barycentric coordinate
            Vector s2 = Geometry.Cross(d, e1);
            double b2 = Geometry.Dot(r.d, s2) * invDivisor;
            if (b2 < 0.0d || b1 + b2 > 1.0d)
                return false;

            // Compute _t_ to intersection point
            double t = Geometry.Dot(e2, s2) * invDivisor;
            if (t < r.mint || t > r.maxt)
                return false;

            // Test shadow r intersection against alpha texture, if present
            if (r.depth != -1 && mesh.alphaTexture != null)
            {
                // Compute triangle partial derivatives
                Vector dpdu, dpdv;
                double[,] uvs = new double[3, 2];
                GetUVs(uvs);

                // Compute deltas for triangle partial derivatives
                double du1 = uvs[0, 0] - uvs[2, 0];
                double du2 = uvs[1, 0] - uvs[2, 0];
                double dv1 = uvs[0, 1] - uvs[2, 1];
                double dv2 = uvs[1, 1] - uvs[2, 1];
                Vector dp1 = p1 - p3, dp2 = p2 - p3;
                double determinant = du1 * dv2 - dv1 * du2;
                if (determinant == 0.0d)
                {
                    // Handle zero determinant for triangle partial derivative matrix
                    Geometry.CoordinateSystem(Geometry.Normalize(Geometry.Cross(e2, e1)), out dpdu, out dpdv);
                }
                else
                {
                    double invdet = 1.0d / determinant;
                    dpdu = (dv2 * dp1 - dv1 * dp2) * invdet;
                    dpdv = (-du2 * dp1 + du1 * dp2) * invdet;
                }

                // Interpolate $(u,v)$ triangle parametric coordinates
                double b0 = 1 - b1 - b2;
                double tu = b0 * uvs[0, 0] + b1 * uvs[1, 0] + b2 * uvs[2, 0];
                double tv = b0 * uvs[0, 1] + b1 * uvs[1, 1] + b2 * uvs[2, 1];
                DifferentialGeometry dgLocal = new DifferentialGeometry(r.GetPointAt(t), dpdu, dpdv, new Normal(0, 0, 0), new Normal(0, 0, 0), tu, tv, this);
                if (mesh.alphaTexture.Evaluate(dgLocal) == 0.0d)
                    return false;
            }

            return true;
        }

        public override double Area()
        {
            Point p1 = mesh.p[v[0]];
            Point p2 = mesh.p[v[1]];
            Point p3 = mesh.p[v[2]];
            return 0.5d * Geometry.Cross(p2 - p1, p3 - p1).Length();
        }

        public override void GetShadingGeometry(Transform obj2world, DifferentialGeometry dg, out DifferentialGeometry dgShading)
        {
            if (mesh.n == null && mesh.s == null)
            {
                dgShading = dg;
                return;
            }
            // Initialize _Triangle_ shading geometry with _n_ and _s_

            // Compute barycentric coordinates for point
            double[] b = new double[3];

            // Initialize _A_ and _C_ matrices for barycentrics
            double[,] uv = new double[3, 2];
            GetUVs(uv);
            double[,] A = { { uv[1,0] - uv[0,0], uv[2,0] - uv[0,0] },
                          { uv[1,1] - uv[0,1], uv[2,1] - uv[0,1] } };
            double[] C = { dg.u - uv[0, 0], dg.v - uv[0, 1] };
            if (!Utility.SolveLinearSystem2x2(A, C, out b[1], out b[2]))
            {
                // Handle degenerate parametric mapping
                b[0] = b[1] = b[2] = 1.0f / 3.0f;
            }
            else
                b[0] = 1.0d - b[1] - b[2];

            // Use _n_ and _s_ to compute shading tangents for triangle, _ss_ and _ts_
            Normal ns;
            Vector ss, ts;
            if (mesh.n != null) ns = Geometry.Normalize(obj2world.Apply(b[0] * mesh.n[v[0]] +
                                                    b[1] * mesh.n[v[1]] +
                                                    b[2] * mesh.n[v[2]]));
            else ns = dg.nn;
            if (mesh.s != null) ss = Geometry.Normalize(obj2world.Apply(b[0] * mesh.s[v[0]] +
                                                    b[1] * mesh.s[v[1]] +
                                                    b[2] * mesh.s[v[2]]));
            else ss = Geometry.Normalize(dg.dpdu);

            ts = Geometry.Cross(ss, ns);
            if (ts.LengthSquared() > 0.0d)
            {
                ts = Geometry.Normalize(ts);
                ss = Geometry.Cross(ts, ns);
            }
            else
                Geometry.CoordinateSystem(new Vector(ns), out ss, out ts);
            Normal dndu, dndv;

            // Compute $\dndu$ and $\dndv$ for triangle shading geometry
            if (mesh.n != null)
            {
                double[,] uvs = new double[3, 2];
                GetUVs(uvs);
                // Compute deltas for triangle partial derivatives of normal
                double du1 = uvs[0, 0] - uvs[2, 0];
                double du2 = uvs[1, 0] - uvs[2, 0];
                double dv1 = uvs[0, 1] - uvs[2, 1];
                double dv2 = uvs[1, 1] - uvs[2, 1];
                Normal dn1 = mesh.n[v[0]] - mesh.n[v[2]];
                Normal dn2 = mesh.n[v[1]] - mesh.n[v[2]];
                double determinant = du1 * dv2 - dv1 * du2;
                if (determinant == 0.0d)
                    dndu = dndv = new Normal(0, 0, 0);
                else
                {
                    double invdet = 1.0d / determinant;
                    dndu = (dv2 * dn1 - dv1 * dn2) * invdet;
                    dndv = (-du2 * dn1 + du1 * dn2) * invdet;
                }
            }
            else
                dndu = dndv = new Normal(0, 0, 0);
            dgShading = new DifferentialGeometry(dg.p, ss, ts,
                ObjectToWorld.Apply(dndu), ObjectToWorld.Apply(dndv), dg.u, dg.v, dg.shape);
            dgShading.dudx = dg.dudx; dgShading.dvdx = dg.dvdx;
            dgShading.dudy = dg.dudy; dgShading.dvdy = dg.dvdy;
            dgShading.dpdx = dg.dpdx; dgShading.dpdy = dg.dpdy;
        }

        public override Point Sample(double u1, double u2, ref Normal ns)
        {
            double b1, b2;
            MonteCarlo.UniformSampleTriangle(u1, u2, out b1, out b2);
            // Get triangle vertices in _p1_, _p2_, and _p3_
            Point p1 = mesh.p[v[0]];
            Point p2 = mesh.p[v[1]];
            Point p3 = mesh.p[v[2]];
            Point p = b1 * p1 + b2 * p2 + (1.0d - b1 - b2) * p3;
            Normal n = new Normal(Geometry.Cross(p2 - p1, p3 - p1));
            ns = Geometry.Normalize(n);
            if (ReverseOrientation) ns *= -1.0d;
            return p;
        }
    }
}
