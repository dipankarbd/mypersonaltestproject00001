using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public class GeometricPrimitive : Primitive
    {
        private Shape shape;
        private Material material;
        private AreaLight areaLight;

        public GeometricPrimitive(Shape s, Material m, AreaLight a)
        {
            shape = s;
            material = m;
            areaLight = a;
        }

        public override BBox WorldBound()
        {
            return shape.WorldBound();
        }

        public override bool CanIntersect()
        {
            return shape.CanIntersect();
        }

        public override bool Intersect(Ray r, Intersection isect)
        {
            double thit, rayEpsilon;
            if (!shape.Intersect(r, out thit, out rayEpsilon, out isect.dg))
                return false;
            isect.primitive = this;
            isect.WorldToObject = shape.WorldToObject;
            isect.ObjectToWorld = shape.ObjectToWorld;
            isect.shapeId = shape.shapeId;
            isect.primitiveId = primitiveId;
            isect.rayEpsilon = rayEpsilon;
            r.maxt = thit;
            return true;
        }

        public override bool IntersectP(Ray r)
        {
            return shape.IntersectP(r);
        }

        public override void Refine(ref LinkedList<Primitive> refined)
        {
            LinkedList<Shape> r = new LinkedList<Shape>();
            shape.Refine(ref r);
            foreach (Shape s in r)
            {
                GeometricPrimitive gp = new GeometricPrimitive(s, material, areaLight);
                refined.AddLast(gp);
            }
        }
        public override AreaLight GetAreaLight()
        {
            return areaLight;
        }

        public override BSDF GetBSDF(DifferentialGeometry dg, Transform ObjectToWorld)
        {
            DifferentialGeometry dgs;
            shape.GetShadingGeometry(ObjectToWorld, dg, out dgs);
            return material.GetBSDF(dg, dgs);
        }

        public override BSSRDF GetBSSRDF(DifferentialGeometry dg, Transform ObjectToWorld)
        {
            DifferentialGeometry dgs;
            shape.GetShadingGeometry(ObjectToWorld, dg, out dgs);
            return material.GetBSSRDF(dg, dgs);
        }
    }
}
