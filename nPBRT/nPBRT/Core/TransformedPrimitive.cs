using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public class TransformedPrimitive : Primitive
    {
        private Primitive primitive;
        private AnimatedTransform WorldToPrimitive;

        public TransformedPrimitive(Primitive prim, AnimatedTransform w2p)
        {
            primitive = prim;
            WorldToPrimitive = w2p;
        }

        public override BBox WorldBound()
        {
            return WorldToPrimitive.MotionBounds(primitive.WorldBound(), true);
        }

        public override bool Intersect(Ray r, Intersection isect)
        {
            Transform w2p;
            WorldToPrimitive.Interpolate(r.time, out w2p);
            Ray ray = w2p.Apply(r);

            if (!primitive.Intersect(ray, isect))
                return false;
            r.maxt = ray.maxt;
            isect.primitiveId = primitiveId;
            if (!w2p.IsIdentity())
            {
                // Compute world-to-object transformation for instance
                isect.WorldToObject = isect.WorldToObject * w2p;
                isect.ObjectToWorld = Transform.Inverse isect.WorldToObject.Inverse(

                // Transform instance's differential geometry to world space
                Transform PrimitiveToWorld = Inverse(w2p);
                isect->dg.p = PrimitiveToWorld(isect->dg.p);
                isect->dg.nn = Normalize(PrimitiveToWorld(isect->dg.nn));
                isect->dg.dpdu = PrimitiveToWorld(isect->dg.dpdu);
                isect->dg.dpdv = PrimitiveToWorld(isect->dg.dpdv);
                isect->dg.dndu = PrimitiveToWorld(isect->dg.dndu);
                isect->dg.dndv = PrimitiveToWorld(isect->dg.dndv);
            }
            return true;
        }

        public override bool IntersectP(Ray r)
        {
            throw new NotImplementedException();
        }

        public override AreaLight GetAreaLight()
        {
            throw new NotImplementedException();
        }

        public override BSDF GetBSDF(DifferentialGeometry dg, Transform ObjectToWorld)
        {
            throw new NotImplementedException();
        }

        public override BSSRDF GetBSSRDF(DifferentialGeometry dg, Transform ObjectToWorld)
        {
            throw new NotImplementedException();
        }
    }
}
