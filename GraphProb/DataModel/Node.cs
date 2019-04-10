using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProb.DataModel
{
    public class Node
    {
        
        public int ID; // Index used as ID inside program, 1 less than actual ID on graph.
        public int Color { get; set; }
        public int[] Children; // all children, not reference since not needed for our implementation.
        public IList<Triangle> Triangles; // all neighboring triangles.
        public IList<Node> MutableChildren; // all neighboring nodes that is mutable (not border)
        public int IDDisplay => this.ID + 1; // actual ID


        public Node(int color, int id, int[] children)
        {
            this.ID = id;
            this.Color = color;
            this.Children = children;
            this.Triangles = new List<Triangle>();
            this.MutableChildren = new List<Node>();
        }

        /// <summary>
        /// check for adjacency
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool IsNeighbor(int c) {
            return Array.IndexOf(this.Children, c) != -1;

        }

        /// <summary>
        /// Returns the number of complete triangles in all neighboring faces.
        /// </summary>
        /// <returns></returns>
        public int GetSum() {
            int r = 0;
            foreach (Triangle t in this.Triangles)
            {
                r += t.Color() == Graph.CompleteTriangleValue ? 1 : 0;
            }
            return r;
        }

        public int[] TestColor(int[] colors) {
            int baseSum = this.GetSum();
            int[] r = new int[colors.Length];
            for (int i =0; i<colors.Length;i++) {
                this.Color = colors[i];
                r[i] = this.GetSum();
            }
            this.Color = 0;
            return r; 

        }

        public override string ToString()
        {
            return "ID: " + this.IDDisplay + " color: " + this.Color;
        }
    }
}
