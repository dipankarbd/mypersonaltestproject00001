using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public abstract class Primitive
    {
        public readonly UInt32 primitiveId;
        protected static UInt32 nextprimitiveId;

        public Primitive()
        {
            primitiveId = nextprimitiveId++;
        }

        public abstract BBox WorldBound();
        public abstract bool Intersect(Ray r, Intersection isect);
        public abstract bool IntersectP(Ray r);
        public abstract AreaLight GetAreaLight();
        public abstract BSDF GetBSDF(DifferentialGeometry dg, Transform ObjectToWorld);
        public abstract BSSRDF GetBSSRDF(DifferentialGeometry dg, Transform ObjectToWorld);

        public virtual bool CanIntersect()
        {
            return true;
        }

        public virtual void Refine(ref LinkedList<Primitive> refined)
        {
            throw new NotImplementedException();
        }
        public void FullyRefine(ref LinkedList<Primitive> refined)
        {
            LinkedList<Primitive> todo = new LinkedList<Primitive>();
            todo.AddLast(this);
            while (todo.Count() > 0)
            {
                // Refine last primitive in todo list
                Primitive prim = todo.First();
                todo.RemoveFirst();

                if (prim.CanIntersect())
                    refined.AddLast(prim);
                else
                    prim.Refine(ref todo);
            }
        }


    }
}
