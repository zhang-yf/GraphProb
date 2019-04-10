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
        public string TriID;                                // the 6 char unique ID
        public Node[] Nodes;                                // reference the verticies, including borders.

        /// <summary>
        /// we use multiplication to identify complete triangle, only the result of 30 means complete since the colors are prime number 2,3,5
        /// </summary>
        /// <returns></returns>
        public int Color() {
            int r = 1;
            foreach (Node n in this.Nodes) {

                r = r * n.Color;
            }
            return r;
        }

    }
}
