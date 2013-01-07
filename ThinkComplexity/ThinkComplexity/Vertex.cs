using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkComplexity
{
    public class Vertex
    {
        public string Label { get; set; }

        public Vertex()
        {
            this.Label = string.Empty;
        }
        public Vertex(string label)
        {
            this.Label = label;
        }

        public override string ToString()
        {
            return "Vertex - (" + Label + ")";
        }
    }
}
