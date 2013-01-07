using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkComplexity
{
    public class Graph : Dictionary<Vertex, Dictionary<Vertex, Edge>>
    {
        public List<Vertex> Vertices
        {
            get
            {
                return this.Keys.ToList();
            }
        }
        public List<Edge> Edges
        {
            get
            {
                List<Edge> edges = new List<Edge>();
                foreach (var vOuter in this)
                {
                    foreach (var vInner in vOuter.Value)
                    {
                        if (!edges.Contains(vInner.Value))
                        {
                            edges.Add(vInner.Value);
                        }
                    }
                }
                return edges;
            }
        }

        public Graph()
        {
        }

        public Graph(List<Vertex> vertices)
        {
            foreach (Vertex item in vertices)
            {
                this.AddVertex(item);
            }
        }
        public Graph(List<Vertex> vertices, List<Edge> edges)
        {
            foreach (Vertex item in vertices)
            {
                this.AddVertex(item);
            }
            foreach (Edge item in edges)
            {
                this.AddEdge(item);
            }
        }

        public void AddVertex(Vertex vertex)
        {
            if (!this.ContainsKey(vertex))
            {
                this.Add(vertex, new Dictionary<Vertex, Edge>());
            }
            else
            {
                throw new Exception("Vertex already exists");
            }
        }

        public void AddEdge(Edge edge)
        {
            Vertex v1 = edge.Vertex1;
            Vertex v2 = edge.Vertex2;

            if (this.ContainsKey(v1))
            {
                if (this[v1].ContainsKey(v2))
                {
                    this[v1][v2] = edge;
                }
                else
                {
                    this[v1].Add(v2, edge);
                }
            }
            if (this.ContainsKey(v2))
            {
                if (this[v2].ContainsKey(v1))
                {
                    this[v2][v1] = edge;
                }
                else
                {
                    this[v2].Add(v1, edge);
                }
            }
        }

        public Edge GetEdge(Vertex v1, Vertex v2)
        {
            if (this.ContainsKey(v1))
            {
                if (this[v1].ContainsKey(v2))
                {
                    return this[v1][v2];
                }
            }
            return null;
        }

        public void RemoveEdge(Edge edge)
        {
            Vertex v1 = edge.Vertex1;
            Vertex v2 = edge.Vertex2;
            if (this.ContainsKey(v1))
            {
                if (this[v1].ContainsKey(v2))
                {
                    this[v1].Remove(v2);
                }
            }

            if (this.ContainsKey(v2))
            {
                if (this[v2].ContainsKey(v1))
                {
                    this[v2].Remove(v1);
                }
            }
        }

        public List<Vertex> OutVertices(Vertex vertex)
        {
            if (this.ContainsKey(vertex))
            {
                return this[vertex].Keys.ToList();
            }
            return null;
        }

        public List<Edge> OutEdges(Vertex vertex)
        {
            if (this.ContainsKey(vertex))
            {
                return this[vertex].Values.ToList();
            }
            return null;
        }

        public void AddAllEdges()
        {
            List<Vertex> vertices = Vertices;
            for (int i = 0; i < vertices.Count; i++)
            {
                Vertex v1 = vertices[i];
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (i != j)
                    {
                        Vertex v2 = vertices[j];
                        Edge edge = new Edge(v1, v2);
                        this.AddEdge(edge);
                    }
                }
            } 
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var item in this)
            {
                sb.Append(item.Key.ToString() + " : ");
                sb.Append("{");
                foreach (var itemInner in item.Value)
                {
                    sb.Append(itemInner.Key.ToString() + " : {" + itemInner.Value.ToString() + "}, ");
                }
                sb.Append("}, ");
            }
            sb.Append("}");
            return sb.ToString();
        }


    }
}
