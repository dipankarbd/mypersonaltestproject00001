using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nPBRT.Core;

namespace nPBRT.Shapes
{
    public class TriangleMesh : Shape
    {
        internal int ntris, nverts;
        internal int[] vertexIndex;
        internal Point[] p;
        internal Normal[] n;
        internal Vector[] s;
        internal double[] uvs;
        internal Texture<double> alphaTexture;


        public TriangleMesh(Transform o2w, Transform w2o, bool ro, int nt, int nv, int[] vi, Point[] P, Normal[] N, Vector[] S, double[] uv, Texture<double> atex)
            : base(o2w, w2o, ro)
        {
            alphaTexture = atex;
            ntris = nt;
            nverts = nv;
            vertexIndex = new int[ntris * 3];
            Array.Copy(vi, 0, vertexIndex, 0, ntris * 3);
            // Copy _uv_, _N_, and _S_ vertex data, if present
            if (uv != null)
            {
                uvs = new double[2 * nverts];
                Array.Copy(uv, 0, uvs, 0, 2 * nverts);
            }
            else
            {
                uvs = null;
            }
            p = new Point[nverts];
            if (N != null)
            {
                n = new Normal[nverts];
                Array.Copy(N, 0, n, 0, nverts);
            }
            else
            {
                n = null;
            }
            if (S != null)
            {
                s = new Vector[nverts];
                Array.Copy(S, 0, s, 0, nverts);
            }
            else
            {
                s = null;
            }
            // Transform mesh vertices to world space
            for (int i = 0; i < nverts; ++i)
            {
                p[i] = ObjectToWorld.Apply(P[i]);
            }
        }


        public override BBox ObjectBound()
        {
            BBox objectBounds = new BBox();
            for (int i = 0; i < nverts; i++)
                objectBounds = Geometry.Union(objectBounds, WorldToObject.Apply(p[i]));
            return objectBounds;
        }

        public override BBox WorldBound()
        {
            BBox worldBounds = new BBox();
            for (int i = 0; i < nverts; i++)
                worldBounds = Geometry.Union(worldBounds, p[i]);
            return worldBounds;
        }

        public override bool CanIntersect()
        {
            return false;
        }

        public override void Refine(ref LinkedList<Shape> refined)
        {
            for (int i = 0; i < ntris; ++i)
                refined.AddLast(new Triangle(ObjectToWorld, WorldToObject, ReverseOrientation, this, i));
        }
        public override bool Intersect(Ray r, out double tHit, out double rayEpsilon, out DifferentialGeometry dg)
        {
            throw new NotImplementedException();
        }

        public override bool IntersectP(Ray r)
        {
            throw new NotImplementedException();
        }

        public override double Area()
        {
            throw new NotImplementedException();
        }

        public static TriangleMesh CreateTriangleMeshShape(Transform o2w, Transform w2o, bool reverseOrientation, ParamSet parameters, Dictionary<string, Texture<double>> floatTextures)
        {
            throw new NotImplementedException();
            //int nvi, npi, nuvi, nsi, nni;
            //const int *vi = parameters.FindInt("indices", &nvi);
            //const Point *P = parameters.FindPoint("P", &npi);
            //const double *uvs = parameters.FindFloat("uv", &nuvi);
            //if (!uvs) uvs = parameters.FindFloat("st", &nuvi);
            //bool discardDegnerateUVs = parameters.FindOneBool("discarddegenerateUVs", false);
            //// XXX should complain if uvs aren't an array of 2...
            //if (uvs) {
            //    if (nuvi < 2 * npi) {
            //        Error("Not enough of \"uv\"s for triangle mesh.  Expencted %d, "
            //              "found %d.  Discarding.", 2*npi, nuvi);
            //        uvs = NULL;
            //    }
            //    else if (nuvi > 2 * npi)
            //        Warning("More \"uv\"s provided than will be used for triangle "
            //                "mesh.  (%d expcted, %d found)", 2*npi, nuvi);
            //}
            //if (!vi || !P) return NULL;
            //const Vector *S = parameters.FindVector("S", &nsi);
            //if (S && nsi != npi) {
            //    Error("Number of \"S\"s for triangle mesh must match \"P\"s");
            //    S = NULL;
            //}
            //const Normal *N = parameters.FindNormal("N", &nni);
            //if (N && nni != npi) {
            //    Error("Number of \"N\"s for triangle mesh must match \"P\"s");
            //    N = NULL;
            //}
            //if (discardDegnerateUVs && uvs && N) {
            //    // if there are normals, check for bad uv's that
            //    // give degenerate mappings; discard them if so
            //    const int *vp = vi;
            //    for (int i = 0; i < nvi; i += 3, vp += 3) {
            //        double area = .5f * Cross(P[vp[0]]-P[vp[1]], P[vp[2]]-P[vp[1]]).Length();
            //        if (area < 1e-7) continue; // ignore degenerate tris.
            //        if ((uvs[2*vp[0]] == uvs[2*vp[1]] &&
            //            uvs[2*vp[0]+1] == uvs[2*vp[1]+1]) ||
            //            (uvs[2*vp[1]] == uvs[2*vp[2]] &&
            //            uvs[2*vp[1]+1] == uvs[2*vp[2]+1]) ||
            //            (uvs[2*vp[2]] == uvs[2*vp[0]] &&
            //            uvs[2*vp[2]+1] == uvs[2*vp[0]+1])) {
            //            Warning("Degenerate uv coordinates in triangle mesh.  Discarding all uvs.");
            //            uvs = NULL;
            //            break;
            //        }
            //    }
            //}
            //for (int i = 0; i < nvi; ++i)
            //    if (vi[i] >= npi) {
            //        Error("trianglemesh has out of-bounds vertex index %d (%d \"P\" values were given",
            //            vi[i], npi);
            //        return NULL;
            //    }

            //Reference<Texture<double> > alphaTex = NULL;
            //string alphaTexName = params.FindTexture("alpha");
            //if (alphaTexName != "") {
            //    if (floatTextures->find(alphaTexName) != floatTextures->end())
            //        alphaTex = (*floatTextures)[alphaTexName];
            //    else
            //        Error("Couldn't find double texture \"%s\" for \"alpha\" parameter",
            //              alphaTexName.c_str());
            //}
            //else if (params.FindOneFloat("alpha", 1.f) == 0.f)
            //    alphaTex = new ConstantTexture<double>(0.f);
            //return new TriangleMesh(o2w, w2o, reverseOrientation, nvi/3, npi, vi, P,
            //    N, S, uvs, alphaTex);
        }
    }
}
