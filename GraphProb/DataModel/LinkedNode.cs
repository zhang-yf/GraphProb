using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProb.DataModel
{
    /// <summary>
    /// This class Wraps around a Node object since we don't want to make copies of actually Node obect.
    /// Could Definitly convert into a interface.
    /// Used for re-index nodes for efficient traversal.
    /// </summary>
    public class LinkedNode 
    {
      
        public LinkedNode(Node n) {
            this._Node = n;

        }
        Node _Node { get; set; }

        public int Color { get { return this._Node.Color; } set { _Node.Color = value; } }
        public int GetSum() { return this._Node.GetSum();  }

        public LinkedNode Next { get; set; }
    }
}
