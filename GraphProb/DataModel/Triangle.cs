using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProb.DataModel
{
    public class Triangle
    {
        public Triangle(string id)
        {
            this.TriID = id;
            this.Nodes = new Node[3];
        }
        public string TriID;
        public Node[] Nodes;

        public int Color() {
            int r = 1;
            foreach (Node n in this.Nodes) {

                r = r * n.Color;
            }
            return r;
        }

    }
}
