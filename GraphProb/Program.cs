using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GraphProb
{
    using GraphProb.DataModel;
    class Program
    {
        static void Main(string[] args)
        {

            Graph g = new Graph();
            bool result = false;

            Console.WriteLine("Starting First Algorithm ...");
            DateTime FirstStart = DateTime.Now;
           
            result= g.RecursiveTraversal(g.DynamicNodes.First(), 0);
            DateTime FirstResult = DateTime.Now;
            Console.WriteLine("First Algorithm took " + (FirstResult - FirstStart).TotalMilliseconds + "ms, the result is: " + result.ToString());
            g.Reset();
            DateTime SecondStart = DateTime.Now;
            
            result = g.AllPossibleColor();

            DateTime SecondResult = DateTime.Now;
            Console.WriteLine("Second Algorithm took " + (SecondResult - SecondStart).TotalMilliseconds + "ms, the result is: " + result.ToString());

            Console.WriteLine("Program Finished.");
            Console.ReadKey();
        }
    }
}
