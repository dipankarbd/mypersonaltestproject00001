using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nPBRT.Core
{
    public class Material
    {
        public BSDF GetBSDF(DifferentialGeometry dg, DifferentialGeometry dgs)
        {
            throw new NotImplementedException();
        }

        public BSSRDF GetBSSRDF(DifferentialGeometry dg, DifferentialGeometry dgs)
        {
            throw new NotImplementedException();
        }
    }
}
