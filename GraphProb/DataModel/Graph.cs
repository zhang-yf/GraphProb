using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProb.DataModel
{
    public class Graph
    {

        private Node[] AllNodes;
        public IList<Node> DynamicNodes { get; set; }
        private IDictionary<string, Triangle> Triangles { get; set; }
        private IList<LinkedNode> LinkedNodes { get; set; }

        private readonly int[] Colors = new int[3] {2,3,5};
        public static int count =0;

        public const int CompleteTriangleValue = 30;



        public Graph(bool readFromFile = true,string textFile = "C:\\Users\\yifengz\\source\\repos\\GraphProb\\GraphProb\\Graph.txt") {
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



        private void GenerateTriangleAndMutibleChildren() {
            for (int n = 0; n<this.AllNodes.Length;n++) {
                int c2 = this.AllNodes[n].Children[this.AllNodes[n].Children.Length - 1];

                for (int i =0; i < this.AllNodes[n].Children.Length;i++) {
                    int c = this.AllNodes[n].Children[i];
                    if (this.IsTriangle(n, c, c2)) {
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


        private bool IsTriangle(int a, int b, int c) {
            return this.AllNodes[a].IsNeighbor(b) && this.AllNodes[a].IsNeighbor(c) && this.AllNodes[b].IsNeighbor(c);

        }

        public bool RecursiveTraversal( Node n ,int sum) {

            
            int[] testResult = n.TestColor(this.Colors);
            count += 1;

            for (int i=0;i<testResult.Length;i++) {
                if (testResult[i] + sum <= 2) {
                    n.Color = this.Colors[i];
                    bool thereIsLessThanTwoTriangle = false;
                    foreach (Node child in n.MutableChildren) {
                        if (child.Color == 0)
                        {
                            
                            thereIsLessThanTwoTriangle |= this.RecursiveTraversal(child, sum + testResult[i]);
                        }
                        else {
                            
                        }
                    }
                    if (thereIsLessThanTwoTriangle) {
                        return true;
                    }
                }

            }
            //Console.WriteLine(n.ToString() + " sum :" + sum);
            n.Color = 0;
            return false;
            
        }


        public bool AllPossibleColor() {
            bool answer = false;
            
            int maximumSumOfColors = this.LinkedNodes.Count * this.Colors[this.Colors.Length-1];
            foreach (LinkedNode n in this.LinkedNodes) {
                n.Color = 2;
                
            }
            int sumOfColors = this.LinkedNodes.Count * this.Colors[0];

            int numOfCompleteTriangle = this.Evaluate();
            //int count = 1;

            if (numOfCompleteTriangle < 3) {
                return true;
            }

            while (sumOfColors < maximumSumOfColors) {

                
                LinkedNode first = this.LinkedNodes.First();
                int prevSum = sumOfColors;

                int localNumOfTriangle = first.getSum();
                sumOfColors += this.IncrementColor(first);
                numOfCompleteTriangle += (first.getSum() - localNumOfTriangle);

                while (prevSum > sumOfColors) {
                    
                    first = first.Next;
                    localNumOfTriangle = first.getSum();
                    prevSum = sumOfColors;
                    sumOfColors += this.IncrementColor(first);
                    numOfCompleteTriangle += (first.getSum() - localNumOfTriangle);
                }
                //count++;
                if(numOfCompleteTriangle < 3)
                {
                    return true;
                }


            }

            //Console.WriteLine(count);

            return answer;

        }

        private int ColorIndex(int color) {
            int i= 0;
            while (this.Colors[i] != color && i < this.Colors.Length) {
                i++;
            }
            return i<this.Colors.Length? i: -1;

        }

        private int IncrementColor(LinkedNode n) {

            int index = this.ColorIndex(n.Color);
            n.Color = this.Colors[(index + 1) % this.Colors.Length];
            return this.Colors[(index + 1) % this.Colors.Length] - this.Colors[index];


        }

        private int Evaluate() {
            int sum = 0;
            foreach (Triangle t in this.Triangles.Values) {
                sum += t.Color() == CompleteTriangleValue ? 1 : 0;

            }
            return sum;

        }

        public void Reset() {

            foreach (Node n in this.DynamicNodes) {
                n.Color = 0;
            }
        }

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
