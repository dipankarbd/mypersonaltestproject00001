using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkComplexity
{
    public class GraphWorld : Canvas
    {
        Graph graph;
        Layout layout;

        int xOffset;
        int yOffset;

        public GraphWorld(Graph graph, Layout layout)
        {
            this.graph = graph;
            this.layout = layout;
            this.Text = "Graph World";
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            xOffset = -10 + this.Width / 2;
            yOffset = -20 + this.Height / 2;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var item in graph.Edges)
            {
                DrawEdge(e, item);
            }
            foreach (var item in layout)
            {
                DrawVertex(e, item.Key, item.Value);
            }
        }

        private void DrawEdge(System.Windows.Forms.PaintEventArgs e, Edge edge)
        {
            Pen p = new Pen(Brushes.DarkOliveGreen, 1);
            e.Graphics.DrawLine(p, GetPoint(layout[edge.Vertex1]), GetPoint(layout[edge.Vertex2]));
        }
        private Point GetPoint(VertexPosition pos)
        {
            return new Point(xOffset + (int)pos.x, yOffset + (int)pos.y);
        }
        private void DrawVertex(System.Windows.Forms.PaintEventArgs e, Vertex vertex, VertexPosition vertexPosition)
        {
            int r = random.Next(50, 55);

            Pen p = new Pen(Brushes.DarkOliveGreen, 2);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawEllipse(p, new Rectangle((int)vertexPosition.x - (r / 2) + xOffset, (int)vertexPosition.y - (r / 2) + yOffset, r, r));
            e.Graphics.FillEllipse(Brushes.OliveDrab, new Rectangle((int)vertexPosition.x - (r / 2) + xOffset, (int)vertexPosition.y - (r / 2) + yOffset, r, r));
          
            e.Graphics.DrawString(vertex.Label, SystemFonts.CaptionFont, Brushes.White, new RectangleF(vertexPosition.x - (r / 2) + xOffset, vertexPosition.y - (r / 2) + yOffset, r, r), stringFormat);
        }
    }
    public struct VertexPosition
    {
        public float x;
        public float y;

        public VertexPosition(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
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

        public void SortByDistance(Vertex v, List<Vertex> others)
        {
            foreach (var w in others)
            {
                float t = this.DistanceBetween(v, w);
            }
            //    """Returns a list of the vertices in others sorted in
            //    increasing order by their distance from v."""
            //    t = [(self.distance_between(v, w), w) for w in others]
            //    t.sort()
            //    return [w for (d, w) in t]
            throw new NotImplementedException();
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
}
