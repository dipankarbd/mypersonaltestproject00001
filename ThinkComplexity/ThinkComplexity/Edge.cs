using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkComplexity
{
    public class Edge
    {
        public Vertex Vertex1 { get; set; }
        public Vertex Vertex2 { get; set; }
        public Edge()
        {
            Vertex1 = new Vertex();
            Vertex2 = new Vertex();
        }
        public Edge(Vertex v1, Vertex v2)
        {
            Vertex1 = v1;
            Vertex2 = v2;
        }
        public override string ToString()
        {
            return "Edge - (" + Vertex1.ToString() + "," + Vertex2.ToString() + ")";
        }
    }
}
