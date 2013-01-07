using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Collections;


namespace ThinkComplexity
{
    public class RandomGraph : Graph
    {
        Random random = new Random();
        public RandomGraph()
        {

        }

        public RandomGraph(List<Vertex> vertices)
            : base(vertices)
        {

        }

        public void AddRandomEdges(float p = 0.5f)
        {

            var vs = this.Vertices;
            for (int i = 0; i < vs.Count; i++)
            {
                Vertex v = vs[i];
                for (int j = 0; j < vs.Count; j++)
                {
                    Vertex w = vs[j];
                    if (j <= i) continue;
                    if (random.NextDouble() > p) continue;
                    AddEdge(new Edge(v, w));

                }
            }
        }
        public List<Vertex> BSF(Vertex s, bool visit = false)
        {
            List<Vertex> visited = new List<Vertex>();

            Deque queue = new Deque(new List<Vertex>() { s });
            while (queue.Count > 0)
            {
                Vertex v = queue.PopBack() as Vertex;
                if (visited.Contains(v)) continue;

                visited.Add(v);

                //     visited.add(v)
                ////    if visit: visit(v)

                List<Vertex> outVertices = OutVertices(v);
                foreach (var item in outVertices)
                {
                    queue.PushFront(item);
                }
            }

            return visited;
        }
        public bool IsConnected()
        {
            var vs = this.Vertices;
            var visited = this.BSF(vs[0]);
            return visited.Count == vs.Count;
        }
    }
}
