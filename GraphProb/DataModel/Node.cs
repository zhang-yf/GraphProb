using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProb.DataModel
{
    public class Node
    {
        public Node(int color,int id, int[] children) {
            this.ID = id;
            this.Color = color;
            this.Children = children;
            this.Triangles = new List<Triangle>();
            this.MutableChildren = new List<Node>();
        }
        public int ID;
        public int Color { get; set; }
        public int[] Children;
        public IList<Triangle> Triangles;
        public IList<Node> MutableChildren;
        public int IDDisplay => this.ID + 1;
        
        public bool IsNeighbor(int c) {
            return Array.IndexOf(this.Children, c) != -1;

        }

        public int getSum() {
            int r = 0;
            foreach (Triangle t in this.Triangles)
            {
                r += t.Color() == Graph.CompleteTriangleValue ? 1 : 0;
            }
            return r;
        }

        public int[] TestColor(int[] colors) {
            int baseSum = this.getSum();
            int[] r = new int[colors.Length];
            for (int i =0; i<colors.Length;i++) {
                this.Color = colors[i];
                r[i] = this.getSum();
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
