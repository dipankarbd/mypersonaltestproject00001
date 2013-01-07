using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkComplexity
{
    class Program
    {
        static void Main(string[] args)
        {
            //Vertex v = new Vertex("v");
            //Vertex w = new Vertex("w");
            //Edge e = new Edge(v, w);
            //Graph g = new Graph(new List<Vertex>() { v, w }, new List<Edge>() { e });

            //Console.WriteLine(g.ToString());

            ////Console.WriteLine(g.GetEdge(v, v) == null ? "NULL" : g.GetEdge(v, v).ToString());
            //g.RemoveEdge(e);
             
            //Console.WriteLine(g.ToString());

            int n = 20;
            List<Vertex> vertices = new List<Vertex>();
            for (int i = 0; i < n; i++)
            {
                vertices.Add(new Vertex("V " + (i + 1)));
            }
            
            //Graph g = new Graph(vertices);
            //g.AddAllEdges();
            //RandomLayout layout = new RandomLayout(g,400);
            //layout.SpreadVertices();
            //GraphWorld gw = new GraphWorld(g,layout);
            //gw.ShowDialog();

            RandomGraph g = new RandomGraph(vertices);
            g.AddRandomEdges(0.5f);
            RandomLayout layout = new RandomLayout(g, 400);
             layout.SpreadVertices();
            GraphWorld gw = new GraphWorld(g, layout);
            gw.ShowDialog();

            Console.ReadKey();
        }
    }
}
