using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace nPBRT.Core
{
    public abstract class Shape
    {
        public Transform ObjectToWorld, WorldToObject;
        public bool ReverseOrientation, TransformSwapsHandedness;
        public UInt32 shapeId;
        public static UInt32 nextshapeId;

        public Shape(Transform o2w, Transform w2o, bool ro)
        {
            this.ObjectToWorld = o2w;
            this.WorldToObject = w2o;
            this.ReverseOrientation = ro;
            this.TransformSwapsHandedness = o2w.SwapsHandedness();
            this.shapeId = nextshapeId++;
        }

        public abstract BBox ObjectBound(); 
        public abstract bool Intersect(Ray r, out double tHit, out double rayEpsilon, out DifferentialGeometry dg);
        public abstract bool IntersectP(Ray r);
        public abstract double Area();
        public abstract Point Sample(double u1, double u2, ref Normal ns);

        public virtual BBox WorldBound()
        {
            return ObjectToWorld.Apply(ObjectBound());
        }

        public virtual void GetShadingGeometry(Transform obj2world, DifferentialGeometry dg, out DifferentialGeometry dgShading)
        {
            dgShading = dg;
        }

        public virtual void Refine(ref List<Shape> refined)
        {
            throw new NotImplementedException();
        }

        public virtual Point Sample(Point p, double u1, double u2, ref Normal ns)
        {
            return Sample(u1, u2, ref ns);
        }
         
        public virtual double Pdf(Point Pshape)
        {
            return 1.0d / Area();
        }
         
        public virtual double Pdf(Point p, Vector wi)
        {
            // Intersect sample ray with area light geometry
            DifferentialGeometry dgLight;
            Ray ray = new Ray(p, wi, 1e-3d);
            ray.depth = -1; // temporary hack to ignore alpha mask
            double thit, rayEpsilon;
            if (!Intersect(ray, out thit, out rayEpsilon, out dgLight)) return 0.0d;

            // Convert light sample weight to solid angle measure
            double pdf =  Geometry.DistanceSquared(p, ray.GetPointAt(thit)) / ( Geometry.AbsDot(dgLight.nn, -wi) * Area());
            if (Double.IsInfinity(pdf)) pdf = 0.0d;
            return pdf;
        }

        public bool CanIntersect()
        {
            return true;
        }
        
    }
}
