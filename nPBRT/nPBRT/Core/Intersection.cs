using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public class Intersection
    {
        public DifferentialGeometry dg;
        public Primitive primitive;
        public Transform WorldToObject, ObjectToWorld;
        public UInt32 shapeId, primitiveId;
        public double rayEpsilon;

        public Intersection()
        {
            dg = new DifferentialGeometry();
        }

        public BSDF GetBSDF(RayDifferential ray)
        {
            dg.ComputeDifferentials(ray);
            BSDF bsdf = primitive.GetBSDF(dg, ObjectToWorld);
            return bsdf;
        }
        public BSSRDF GetBSSRDF(RayDifferential ray)
        {
            dg.ComputeDifferentials(ray);
            BSSRDF bssrdf = primitive.GetBSSRDF(dg, ObjectToWorld);
            return bssrdf;
        }
        public Spectrum Le(Vector w)
        {
            AreaLight area = primitive.GetAreaLight();
            return area != null ? area.L(dg.p, dg.nn, w) : new Spectrum(0.0d);
        }
    }
}
