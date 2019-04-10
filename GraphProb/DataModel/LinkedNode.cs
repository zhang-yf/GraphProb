using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProb.DataModel
{
    public class LinkedNode 
    {
      
        public LinkedNode(Node n) {
            this._Node = n;

        }
        Node _Node { get; set; }

        public int Color { get { return this._Node.Color; } set { _Node.Color = value; } }
        public int getSum() { return this._Node.getSum();  }

        public LinkedNode Next { get; set; }
    }
}
