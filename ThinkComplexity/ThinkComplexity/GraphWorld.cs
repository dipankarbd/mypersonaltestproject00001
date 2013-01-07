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
}
