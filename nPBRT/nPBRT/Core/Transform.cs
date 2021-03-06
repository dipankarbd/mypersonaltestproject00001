﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public class Transform
    {
        private Matrix4x4 m, mInv;
        public Matrix4x4 M { get { return m; } }
        public Transform()
        {

        }
        public Transform(double[,] mat)
        {
            this.m = new Matrix4x4(mat[0, 0], mat[0, 1], mat[0, 2], mat[0, 3],
                          mat[1, 0], mat[1, 1], mat[1, 2], mat[1, 3],
                          mat[2, 0], mat[2, 1], mat[2, 2], mat[2, 3],
                          mat[3, 0], mat[3, 1], mat[3, 2], mat[3, 3]);
            this.mInv = Inverse(this.m);
        }
        public Transform(Matrix4x4 mat)
        {
            this.m = mat;
            this.mInv = Inverse(mat);
        }
        public Transform(Matrix4x4 mat, Matrix4x4 minv)
        {
            this.m = mat;
            this.mInv = minv;
        }
        public virtual Transform Inverse(Transform t)
        {
            return new Transform(t.mInv, t.m);
        }


        public virtual Transform Transpose(Transform t)
        {
            return new Transform(Transpose(t.m), Transpose(t.mInv));
        }
        public Point Apply(Point pt)
        {
            double x = pt.x, y = pt.y, z = pt.z;
            double xp = m.m[0, 0] * x + m.m[0, 1] * y + m.m[0, 2] * z + m.m[0, 3];
            double yp = m.m[1, 0] * x + m.m[1, 1] * y + m.m[1, 2] * z + m.m[1, 3];
            double zp = m.m[2, 0] * x + m.m[2, 1] * y + m.m[2, 2] * z + m.m[2, 3];
            double wp = m.m[3, 0] * x + m.m[3, 1] * y + m.m[3, 2] * z + m.m[3, 3];
            if (wp == 0.0d) throw new InvalidOperationException();
            if (wp == 1.0) return new Point(xp, yp, zp);
            else return new Point(xp, yp, zp) / wp;
        }
        public void Apply(Point pt, ref Point ptrans)
        {
            double x = pt.x, y = pt.y, z = pt.z;
            ptrans.x = m.m[0, 0] * x + m.m[0, 1] * y + m.m[0, 2] * z + m.m[0, 3];
            ptrans.y = m.m[1, 0] * x + m.m[1, 1] * y + m.m[1, 2] * z + m.m[1, 3];
            ptrans.z = m.m[2, 0] * x + m.m[2, 1] * y + m.m[2, 2] * z + m.m[2, 3];
            double w = m.m[3, 0] * x + m.m[3, 1] * y + m.m[3, 2] * z + m.m[3, 3];
            if (w != 1.0d) ptrans /= w;
        }
        public Vector Apply(Vector v)
        {
            double x = v.x, y = v.y, z = v.z;
            return new Vector(m.m[0, 0] * x + m.m[0, 1] * y + m.m[0, 2] * z,
                          m.m[1, 0] * x + m.m[1, 1] * y + m.m[1, 2] * z,
                          m.m[2, 0] * x + m.m[2, 1] * y + m.m[2, 2] * z);
        }
        public void Apply(Vector v, ref Vector vt)
        {
            double x = v.x, y = v.y, z = v.z;
            vt.x = m.m[0, 0] * x + m.m[0, 1] * y + m.m[0, 2] * z;
            vt.y = m.m[1, 0] * x + m.m[1, 1] * y + m.m[1, 2] * z;
            vt.z = m.m[2, 0] * x + m.m[2, 1] * y + m.m[2, 2] * z;
        }
        public Normal Apply(Normal n)
        {
            double x = n.x, y = n.y, z = n.z;
            return new Normal(mInv.m[0, 0] * x + mInv.m[1, 0] * y + mInv.m[2, 0] * z,
                          mInv.m[0, 1] * x + mInv.m[1, 1] * y + mInv.m[2, 1] * z,
                          mInv.m[0, 2] * x + mInv.m[1, 2] * y + mInv.m[2, 2] * z);
        }
        public void Apply(Normal n, ref Normal nt)
        {
            double x = n.x, y = n.y, z = n.z;
            nt.x = mInv.m[0, 0] * x + mInv.m[1, 0] * y + mInv.m[2, 0] * z;
            nt.y = mInv.m[0, 1] * x + mInv.m[1, 1] * y + mInv.m[2, 1] * z;
            nt.z = mInv.m[0, 2] * x + mInv.m[1, 2] * y + mInv.m[2, 2] * z;
        }
        public Ray Apply(Ray r)
        {
            Ray ret = r.Copy();
            ret.o = Apply(r.o);
            ret.d = Apply(r.d);
            return ret;
        }
        public BBox Apply(BBox b)
        {
            BBox ret = new BBox(Apply(new Point(b.pMin.x, b.pMin.y, b.pMin.z)));
            ret = Geometry.Union(ret, Apply(new Point(b.pMax.x, b.pMin.y, b.pMin.z)));
            ret = Geometry.Union(ret, Apply(new Point(b.pMin.x, b.pMax.y, b.pMin.z)));
            ret = Geometry.Union(ret, Apply(new Point(b.pMin.x, b.pMin.y, b.pMax.z)));
            ret = Geometry.Union(ret, Apply(new Point(b.pMin.x, b.pMax.y, b.pMax.z)));
            ret = Geometry.Union(ret, Apply(new Point(b.pMax.x, b.pMax.y, b.pMin.z)));
            ret = Geometry.Union(ret, Apply(new Point(b.pMax.x, b.pMin.y, b.pMax.z)));
            ret = Geometry.Union(ret, Apply(new Point(b.pMax.x, b.pMax.y, b.pMax.z)));
            return ret;
        }

        public static Transform operator *(Transform t1, Transform t2)
        {
            Matrix4x4 m1 = Matrix4x4.Mul(t1.m, t2.m);
            Matrix4x4 m2 = Matrix4x4.Mul(t2.mInv, t1.mInv);
            return new Transform(m1, m2);
        }
        public bool SwapsHandedness()
        {
            double det = ((m.m[0, 0] *
                          (m.m[1, 1] * m.m[2, 2] -
                           m.m[1, 2] * m.m[2, 1])) -
                          (m.m[0, 1] *
                          (m.m[1, 0] * m.m[2, 2] -
                           m.m[1, 2] * m.m[2, 0])) +
                          (m.m[0, 2] *
                          (m.m[1, 0] * m.m[2, 1] -
                           m.m[1, 1] * m.m[2, 0])));
            return det < 0.0d;
        }




        public static Matrix4x4 Transpose(Matrix4x4 m)
        {
            return new Matrix4x4(m.m[0, 0], m.m[1, 0], m.m[2, 0], m.m[3, 0],
                             m.m[0, 1], m.m[1, 1], m.m[2, 1], m.m[3, 1],
                             m.m[0, 2], m.m[1, 2], m.m[2, 2], m.m[3, 2],
                             m.m[0, 3], m.m[1, 3], m.m[2, 3], m.m[3, 3]);
        }
        public static Matrix4x4 Inverse(Matrix4x4 m)
        {
            int[] indxc = new int[4];
            int[] indxr = new int[4];
            int[] ipiv = { 0, 0, 0, 0 };
            double[,] minv = new double[4, 4];
            Array.Copy(m.m, minv, 4 * 4);

            for (int i = 0; i < 4; i++)
            {
                int irow = -1, icol = -1;
                double big = 0.0d;
                // Choose pivot
                for (int j = 0; j < 4; j++)
                {
                    if (ipiv[j] != 1)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if (ipiv[k] == 0)
                            {
                                if (Math.Abs(minv[j, k]) >= big)
                                {
                                    big = (Math.Abs(minv[j, k]));
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (ipiv[k] > 1)
                                throw new Exception("Singular matrix in MatrixInvert");
                        }
                    }
                }
                ++ipiv[icol];
                // Swap rows _irow_ and _icol_ for pivot
                if (irow != icol)
                {
                    for (int k = 0; k < 4; ++k)
                        Utility.Swap<double>(ref minv[irow, k], ref  minv[icol, k]);
                }
                indxr[i] = irow;
                indxc[i] = icol;
                if (minv[icol, icol] == 0.0d)
                    throw new Exception("Singular matrix in MatrixInvert");

                // Set $m[icol][icol]$ to one by scaling row _icol_ appropriately
                double pivinv = 1.0d / minv[icol, icol];
                minv[icol, icol] = 1.0d;
                for (int j = 0; j < 4; j++)
                    minv[icol, j] *= pivinv;

                // Subtract this row from others to zero out their columns
                for (int j = 0; j < 4; j++)
                {
                    if (j != icol)
                    {
                        double save = minv[j, icol];
                        minv[j, icol] = 0;
                        for (int k = 0; k < 4; k++)
                            minv[j, k] -= minv[icol, k] * save;
                    }
                }
            }
            // Swap columns to reflect permutation
            for (int j = 3; j >= 0; j--)
            {
                if (indxr[j] != indxc[j])
                {
                    for (int k = 0; k < 4; k++)
                        Utility.Swap<double>(ref minv[k, indxr[j]], ref minv[k, indxc[j]]);
                }
            }
            return new Matrix4x4(minv);
        }

        public static Transform Translate(Vector delta)
        {
            Matrix4x4 m = new Matrix4x4(1, 0, 0, delta.x,
                        0, 1, 0, delta.y,
                        0, 0, 1, delta.z,
                        0, 0, 0, 1);
            Matrix4x4 minv = new Matrix4x4(1, 0, 0, -delta.x,
                           0, 1, 0, -delta.y,
                           0, 0, 1, -delta.z,
                           0, 0, 0, 1);
            return new Transform(m, minv);
        }


        public static Transform Scale(double x, double y, double z)
        {
            Matrix4x4 m = new Matrix4x4(x, 0, 0, 0,
                        0, y, 0, 0,
                        0, 0, z, 0,
                        0, 0, 0, 1);
            Matrix4x4 minv = new Matrix4x4(1.0d / x, 0, 0, 0,
                           0, 1.0d / y, 0, 0,
                           0, 0, 1.0d / z, 0,
                           0, 0, 0, 1);
            return new Transform(m, minv);
        }


        public static Transform RotateX(double angle)
        {
            double sin_t = Math.Sin(Utility.Radians(angle));
            double cos_t = Math.Cos(Utility.Radians(angle));
            Matrix4x4 m = new Matrix4x4(1, 0, 0, 0,
                        0, cos_t, -sin_t, 0,
                        0, sin_t, cos_t, 0,
                        0, 0, 0, 1);
            return new Transform(m, Transpose(m));
        }


        public static Transform RotateY(double angle)
        {
            double sin_t = Math.Sin(Utility.Radians(angle));
            double cos_t = Math.Cos(Utility.Radians(angle));
            Matrix4x4 m = new Matrix4x4(cos_t, 0, sin_t, 0,
                         0, 1, 0, 0,
                        -sin_t, 0, cos_t, 0,
                         0, 0, 0, 1);
            return new Transform(m, Transpose(m));
        }



        public static Transform RotateZ(double angle)
        {

            double sin_t = Math.Sin(Utility.Radians(angle));
            double cos_t = Math.Cos(Utility.Radians(angle));
            Matrix4x4 m = new Matrix4x4(cos_t, -sin_t, 0, 0,
                        sin_t, cos_t, 0, 0,
                        0, 0, 1, 0,
                        0, 0, 0, 1);
            return new Transform(m, Transpose(m));
        }


        public static Transform Rotate(double angle, Vector axis)
        {
            Vector a = Geometry.Normalize(axis);
            double s = Math.Sin(Utility.Radians(angle));
            double c = Math.Cos(Utility.Radians(angle));
            double[,] m = new double[4, 4];

            m[0, 0] = a.x * a.x + (1.0d - a.x * a.x) * c;
            m[0, 1] = a.x * a.y * (1.0d - c) - a.z * s;
            m[0, 2] = a.x * a.z * (1.0d - c) + a.y * s;
            m[0, 3] = 0;

            m[1, 0] = a.x * a.y * (1.0d - c) + a.z * s;
            m[1, 1] = a.y * a.y + (1.0d - a.y * a.y) * c;
            m[1, 2] = a.y * a.z * (1.0d - c) - a.x * s;
            m[1, 3] = 0;

            m[2, 0] = a.x * a.z * (1.0d - c) - a.y * s;
            m[2, 1] = a.y * a.z * (1.0d - c) + a.x * s;
            m[2, 2] = a.z * a.z + (1.0d - a.z * a.z) * c;
            m[2, 3] = 0;

            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = 0;
            m[3, 3] = 1;

            Matrix4x4 mat = new Matrix4x4(m);
            return new Transform(mat, Transpose(mat));
        }


        public static Transform LookAt(Point pos, Point look, Vector up)
        {
            double[,] m = new double[4, 4];
            // Initialize fourth column of viewing matrix
            m[0, 3] = pos.x;
            m[1, 3] = pos.y;
            m[2, 3] = pos.z;
            m[3, 3] = 1;

            // Initialize first three columns of viewing matrix
            Vector dir = Geometry.Normalize(look - pos);
            Vector left = Geometry.Normalize(Geometry.Cross(Geometry.Normalize(up), dir));
            Vector newUp = Geometry.Cross(dir, left);
            m[0, 0] = left.x;
            m[1, 0] = left.y;
            m[2, 0] = left.z;
            m[3, 0] = 0.0d;
            m[0, 1] = newUp.x;
            m[1, 1] = newUp.y;
            m[2, 1] = newUp.z;
            m[3, 1] = 0.0d;
            m[0, 2] = dir.x;
            m[1, 2] = dir.y;
            m[2, 2] = dir.z;
            m[3, 2] = 0.0d;
            Matrix4x4 camToWorld = new Matrix4x4(m);
            return new Transform(Inverse(camToWorld), camToWorld);
        }


        public bool IsIdentity()
        {
            return (m.m[0, 0] == 1.0d && m.m[0, 1] == 0.0d &&
              m.m[0, 2] == 0.0d && m.m[0, 3] == 0.0d &&
              m.m[1, 0] == 0.0d && m.m[1, 1] == 1.0d &&
              m.m[1, 2] == 0.0d && m.m[1, 3] == 0.0d &&
              m.m[2, 0] == 0.0d && m.m[2, 1] == 0.0d &&
              m.m[2, 2] == 1.0d && m.m[2, 3] == 0.0d &&
              m.m[3, 0] == 0.0d && m.m[3, 1] == 0.0d &&
              m.m[3, 2] == 0.0d && m.m[3, 3] == 1.0d);
        }
    }
}
