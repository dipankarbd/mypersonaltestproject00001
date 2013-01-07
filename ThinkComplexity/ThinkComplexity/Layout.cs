using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkComplexity
{
    public class Layout : Dictionary<Vertex, VertexPosition>
    {
        public Layout(Graph graph)
        {
            foreach (var v in graph.Vertices)
            {
                this.Add(v, new VertexPosition(0, 0));
            }
        }
        public VertexPosition Pos(Vertex vertex)
        {
            if (this.ContainsKey(vertex))
            {
                return this[vertex];
            }
            return new VertexPosition();
        }
        public float DistanceBetween(Vertex v1, Vertex v2)
        {
            VertexPosition pos1 = this.Pos(v1);
            VertexPosition pos2 = this.Pos(v2);
            float dx = pos1.x - pos2.x;
            float dy = pos1.y - pos2.y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public List<Vertex> SortByDistance(Vertex v, List<Vertex> others)
        {
            var t = others.OrderBy(obj => this.DistanceBetween(v, obj)).ToList();
            return t;
        }

    }

    public class CircleLayout : Layout
    {
        public CircleLayout(Graph g, float radius = 200)
            : base(g)
        {
            List<Vertex> vs = g.Vertices;
            double theta = Math.PI * 2.0 / vs.Count;
            for (int i = 0; i < vs.Count; i++)
            {
                Vertex v = vs[i];
                double x = radius * Math.Cos(i * theta);
                double y = radius * Math.Sin(i * theta);
                if (this.ContainsKey(v))
                {
                    this[v] = new VertexPosition((float)x, (float)y);
                }
            }
        }
    }
    public class RandomLayout : Layout
    {
        private float max;
        private Random rnd;
        public RandomLayout(Graph g, float max = 10)
            : base(g)
        {
            this.max = max;
            this.rnd = new Random();
            List<Vertex> vs = g.Vertices;
            for (int i = 0; i < vs.Count; i++)
            {
                Vertex v = vs[i];
                if (this.ContainsKey(v))
                {
                    this[v] = this.RandomPos();
                }
            }
        }

        public VertexPosition RandomPos()
        {
            int _max = (int)this.max;
            float x = rnd.Next(-_max, _max);
            float y = rnd.Next(-_max, _max);
            return new VertexPosition(x, y);
        }

        public void SpreadVertex(Vertex v, List<Vertex> others, float min_dist = 100.0f)
        {
            while (true)
            {
                var d = others.Min(obj => DistanceBetween(v, obj));
                if (d > min_dist) break;

                min_dist *= 0.9f;
                if (this.ContainsKey(v))
                {
                    this[v] = this.RandomPos();
                }
            }
        }
        public void SpreadVertices()
        {
            List<Vertex> others = this.Keys.ToList();
            List<Vertex> vs = this.Keys.ToList();
            foreach (var v in vs)
            {
                others.Remove(v);
                this.SpreadVertex(v, others);
                others.Add(v);
            }

        }
    }
}
