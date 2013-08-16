using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core.Geometry
{
    public class DifferentialGeometry
    {
        public Point p;
        public Normal nn;
        public double u, v;
        public Shape shape;
        public Vector dpdu, dpdv;
        public Normal dndu, dndv;
        public Vector dpdx, dpdy;
        public double dudx, dvdx, dudy, dvdy;

        public DifferentialGeometry()
        {
            u = v = dudx = dvdx = dudy = dvdy = 0.0d;
        }

        public DifferentialGeometry(Point P, Vector DPDU, Vector DPDV, Normal DNDU, Normal DNDV, double uu, double vv, Shape sh)
        {
            p = P; dpdu = DPDU; dpdv = DPDV; dndu = DNDU; dndv = DNDV;
            // Initialize _DifferentialGeometry_ from parameters
            nn = new Normal(Geometry.Normalize(Geometry.Cross(dpdu, dpdv)));
            u = uu;
            v = vv;
            shape = sh;
            dudx = dvdx = dudy = dvdy = 0.0d;

            // Adjust normal based on orientation and handedness
            if (shape != null && (shape.ReverseOrientation ^ shape.TransformSwapsHandedness)) nn *= -1.0d;
        }

        public void ComputeDifferentials(RayDifferential ray)
        {
            if (ray.hasDifferentials)
            {
                // Estimate screen space change in $\pt{}$ and $(u,v)$

                // Compute auxiliary intersection points with plane
                double d = -Geometry.Dot(nn, new Vector(p.x, p.y, p.z));
                Vector rxv = new Vector(ray.rxOrigin.x, ray.rxOrigin.y, ray.rxOrigin.z);
                double tx = -(Geometry.Dot(nn, rxv) + d) / Geometry.Dot(nn, ray.rxDirection);
                if (Double.IsNaN(tx))
                {
                    dudx = dvdx = 0.0d;
                    dudy = dvdy = 0.0d;
                    dpdx = dpdy = new Vector(0, 0, 0);
                    return;
                }
                Point px = ray.rxOrigin + tx * ray.rxDirection;
                Vector ryv = new Vector(ray.ryOrigin.x, ray.ryOrigin.y, ray.ryOrigin.z);
                double ty = -(Geometry.Dot(nn, ryv) + d) / Geometry.Dot(nn, ray.ryDirection);
                if (Double.IsNaN(ty))
                {
                    dudx = dvdx = 0.0d;
                    dudy = dvdy = 0.0d;
                    dpdx = dpdy = new Vector(0, 0, 0);
                    return;
                }
                Point py = ray.ryOrigin + ty * ray.ryDirection;
                dpdx = px - p;
                dpdy = py - p;

                // Compute $(u,v)$ offsets at auxiliary points

                // Initialize _A_, _Bx_, and _By_ matrices for offset computation
                double[,] A = new double[2, 2];
                double[] Bx = new double[2];
                double[] By = new double[2];

                int[] axes = new int[2];

                if (Math.Abs(nn.x) > Math.Abs(nn.y) && Math.Abs(nn.x) > Math.Abs(nn.z))
                {
                    axes[0] = 1; axes[1] = 2;
                }
                else if (Math.Abs(nn.y) > Math.Abs(nn.z))
                {
                    axes[0] = 0; axes[1] = 2;
                }
                else
                {
                    axes[0] = 0; axes[1] = 1;
                }

                // Initialize matrices for chosen projection plane
                A[0, 0] = dpdu[axes[0]];
                A[0, 1] = dpdv[axes[0]];
                A[1, 0] = dpdu[axes[1]];
                A[1, 1] = dpdv[axes[1]];
                Bx[0] = px[axes[0]] - p[axes[0]];
                Bx[1] = px[axes[1]] - p[axes[1]];
                By[0] = py[axes[0]] - p[axes[0]];
                By[1] = py[axes[1]] - p[axes[1]];

                if (!Utility.SolveLinearSystem2x2(A, Bx, out dudx, out dvdx))
                {
                    dudx = 0.0d; dvdx = 0.0d;
                }

                if (!Utility.SolveLinearSystem2x2(A, By, out dudy, out dvdy))
                {
                    dudy = 0.0d; dvdy = 0.0d;
                }
            }
            else
            {
                dudx = dvdx = 0.0d;
                dudy = dvdy = 0.0d;
                dpdx = dpdy = new Vector(0, 0, 0);
            }
        }
    }
}
