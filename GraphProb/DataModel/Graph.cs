using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProb.DataModel
{
    /// <summary>
    /// The Class that represents the graph.
    /// The basic requirement is it contains all node and node's immediate children.
    /// </summary>
    public class Graph
    {
        
        
        private readonly int[] Colors = new int[3] {2,3,5};                     // color array, requires to be increment in prime numbers harded coded for now, or initialize with sort.


        //public static int count =0;                                           // this was used for validation, counting the number of traversals. 

        public const int CompleteTriangleValue = 30;                            // 2 * 3 * 5, hard coded for now, but could be derived. this is how we determine if a triangle is complete.

        private Node[] AllNodes;                                                // all nodes includes border nodes
        public IList<Node> DynamicNodes { get; set; }                           // only mutable nodes we are allowed to change color.
        private IDictionary<string, Triangle> Triangles { get; set; }           // all faces, derived from nodes and edges.
        private IList<LinkedNode> LinkedNodes { get; set; }                     // this structure is for second algorithm, could be moved to sub class.






        /// <summary>
        /// Initialize the Graph, read data either through a file or through default static graph embeded in the program
        /// </summary>
        /// <param name="readFromFile"> default false, will read from embeded graph</param>
        /// <param name="textFile"> this is ignored if readFromFile is set to false</param>
        public Graph(bool readFromFile = false,string textFile = "C:\\Users\\yifengz\\sources\\repos\\GraphProb\\GraphProb\\Graph.txt") {
            this.Triangles = new Dictionary<string, Triangle>();
            this.DynamicNodes = new List<Node>();
            this.LinkedNodes = new List<LinkedNode>();
            if (readFromFile) {
                try
                {
                    string content = "";

                    using (StreamReader file = new StreamReader(textFile))
                    {
                        content = file.ReadToEnd();
                    }

                    this.ReadString(content);
                }
                catch (IOException ex) {
                    Console.WriteLine(ex);
                }

            }
            else
            {
                this.ReadString(StaticGraph.Graph);

            }


        }

        /// <summary>
        /// Recursive DFS, traverse the graph using basic DFS. any sub tree is pruned 
        /// when the acumulative complete triangle is more than 3.
        /// 
        /// However, since this is a undirected graph with cycles, DFS performance is very bad (depends on the average degree of nodes and n, bounded by (d^n)^3).
        /// </summary>
        /// <param name="n"> the Root Node</param>
        /// <param name="sum"> The acumulative sum at each internal tree node, usded for pruning.</param>
        /// <returns></returns>
        public bool RecursiveTraversal(Node n, int sum)
        {


            int[] testResult = n.TestColor(this.Colors);
            //count += 1;

            for (int i = 0; i < testResult.Length; i++)
            {
                if (testResult[i] + sum <= 2)
                {
                    n.Color = this.Colors[i];
                    bool thereIsLessThanTwoTriangle = false;
                    foreach (Node child in n.MutableChildren)
                    {
                        if (child.Color == 0)
                        {
                            thereIsLessThanTwoTriangle |= this.RecursiveTraversal(child, sum + testResult[i]);
                        }
                        else
                        {

                        }
                    }
                    if (thereIsLessThanTwoTriangle)
                    {
                        return true;
                    }
                }

            }
            //Console.WriteLine(n.ToString() + " sum :" + sum);
            n.Color = 0;
            return false;

        }




        /// <summary>
        /// Calculate all permutation of the graph with all possible colors and evaluates the 
        /// number of complete triangle at each state. This is designed so that each state change involves one node
        /// and one increment in color so we do not have to calculate the whole graph for complete triangle at each step
        /// rather the result is calculated at each state change. A lot cheaper this way.
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        /// 


        public bool AllPossibleColor()
        {
            
            // the sum of all colors of all vertex is between lowest prime number * n  and  highest prime number * n
            int maximumSumOfColors = this.LinkedNodes.Count * this.Colors[this.Colors.Length - 1];
            foreach (LinkedNode n in this.LinkedNodes)
            {
                n.Color = 2;

            }
            int sumOfColors = this.LinkedNodes.Count * this.Colors[0];

            int numOfCompleteTriangle = this.Evaluate();
            //int count = 1; // for debug/validate

            if (numOfCompleteTriangle < 3)
            {
                return true;
            }
            
            while (sumOfColors < maximumSumOfColors)
            {


                LinkedNode node = this.LinkedNodes.First();
                int prevSum = sumOfColors;

                int localNumOfTriangle = node.getSum();
                sumOfColors += this.IncrementColor(node);
                numOfCompleteTriangle += (node.getSum() - localNumOfTriangle);

                while (prevSum > sumOfColors)
                {

                    node = node.Next;
                    localNumOfTriangle = node.getSum();
                    prevSum = sumOfColors;
                    sumOfColors += this.IncrementColor(node);
                    numOfCompleteTriangle += (node.getSum() - localNumOfTriangle);
                }
                //count++;
                if (numOfCompleteTriangle < 3)
                {
                    return true;
                }


            }

            //Console.WriteLine(count);

            return false;

        }

        /// <summary>
        /// Erase all color except border.
        /// </summary>
        public void Reset()
        {

            foreach (Node n in this.DynamicNodes)
            {
                n.Color = 0;
            }
        }

        /// <summary>
        /// After Graph nodes has been established, generate all triangle faces.
        /// faces is identified by a unique 6 character string consists of ordered node id of its verticies.
        /// This eliminates unique identifier helps prune repeat visits to faces.
        /// Dictionary has built in hash function for query via key, so the seaching using this id is efficient.
        /// Also calculates MutibleChildren for each node, since it belongs to this pre processing step.
        /// </summary>
        private void GenerateTriangleAndMutibleChildren() {
            for (int n = 0; n<this.AllNodes.Length;n++) {

                // interate through all neighbor forms triangle.
                int c2 = this.AllNodes[n].Children[this.AllNodes[n].Children.Length - 1];

                for (int i =0; i < this.AllNodes[n].Children.Length;i++) {
                    int c = this.AllNodes[n].Children[i];
                    if (this.IsTriangle(n, c, c2)) {
                        // sorted vertex ids forms triangle id.
                        int[] temp = new int[3] { n, c, c2 };
                        Array.Sort(temp);
                        string triID =string.Join("", temp.Select(x => x.ToString().PadLeft(2, '0')));

                        if (!this.Triangles.TryGetValue(triID, out Triangle exists)) {
                            Triangle triangle = new Triangle(triID);
                            triangle.Nodes[0] = this.AllNodes[n];
                            triangle.Nodes[1] = this.AllNodes[c];
                            triangle.Nodes[2] = this.AllNodes[c2];
                            this.AllNodes[n].Triangles.Add(triangle);
                            this.AllNodes[c].Triangles.Add(triangle);
                            this.AllNodes[c2].Triangles.Add(triangle);
                            this.Triangles.Add(triID, triangle);
                        }

                    }
                    c2 = c;
                    // ======================== add children to mutible list if color is 0
                    if (this.AllNodes[c].Color == 0) {
                        this.AllNodes[n].MutableChildren.Add(this.AllNodes[c]);

                    }



                }

            }

        }

        /// <summary>
        /// Simple triangle detection used for generating faces
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsTriangle(int a, int b, int c) {
            return this.AllNodes[a].IsNeighbor(b) && this.AllNodes[a].IsNeighbor(c) && this.AllNodes[b].IsNeighbor(c);

        }

        /// <summary>
        /// data driven, used only for Increment color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private int ColorIndex(int color) {
            int i= 0;
            while (this.Colors[i] != color && i < this.Colors.Length) {
                i++;
            }
            return i<this.Colors.Length? i: -1;

        }

        /// <summary>
        /// This Function permuts a node's color to the next color in the color array, it wraps around.
        /// 
        /// Color is encoded as increments of prime numbers, this is important as a lot of optimizaiton depends on that.
        /// It would running into problem if the number of colors become something like in the thousands. since that time
        /// multiplication became costly and we might need to find other ways such as bit maps
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private int IncrementColor(LinkedNode n) {

            int index = this.ColorIndex(n.Color);
            n.Color = this.Colors[(index + 1) % this.Colors.Length];
            return this.Colors[(index + 1) % this.Colors.Length] - this.Colors[index];


        }

        /// <summary>
        /// Calculates total number of complete 
        /// only used once, but could be useful for future enhancement.
        /// </summary>
        /// <returns></returns>
        private int Evaluate() {
            int sum = 0;
            foreach (Triangle t in this.Triangles.Values) {
                sum += t.Color() == CompleteTriangleValue ? 1 : 0;

            }
            return sum;

        }

        
        /// <summary>
        /// Reads a given string containing the whole graph.
        /// Format the graph as the example given in graph.txt
        /// </summary>
        /// <param name="content"></param>
        private void ReadString(string content)
        {

            using (StringReader stringReader = new StringReader(content))
            {

                string line = stringReader.ReadLine();
                while (string.IsNullOrWhiteSpace(line)) {
                    line = stringReader.ReadLine();
                }

                int n = int.Parse(line);
                this.AllNodes = new Node[n];
                for (int i = 0; i < n; i++)
                {

                    string[] fl = stringReader.ReadLine().Split(' ');
                    int[] sl = Array.ConvertAll(stringReader.ReadLine().Split(' '), s => int.Parse(s) - 1);

                    this.AllNodes[i] = new Node(int.Parse(fl[0]), i, sl);
                    if (int.Parse(fl[0]) == 0)
                    {
                        this.DynamicNodes.Add(this.AllNodes[i]);
                        LinkedNode last = this.LinkedNodes.LastOrDefault();
                        LinkedNode next = new LinkedNode(this.AllNodes[i]);
                        if (last != null)
                        {
                            last.Next = next;
                        }
                        this.LinkedNodes.Add(next);

                    }

                }
                this.GenerateTriangleAndMutibleChildren();
            }

        }

    }
}
