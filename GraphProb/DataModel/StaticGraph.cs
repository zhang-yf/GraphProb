﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProb.DataModel
{
    /// <summary>
    /// First line is the number of vertecies.
    /// Followed by 2 lines for each vertex in increment ids (implied).
    /// First line has  Color, number of childrens (not in use)
    /// Second line has an array of childrens by their ids.
    /// </summary>
    public static class StaticGraph
    {
        public const string Graph = @"
22
2 3
2 5 6
2 4
1 6 7 3
5 4
2 7 8 4
3 3
3 8 13
3 4
1 6 22 9
0 5
1 2 5 22 7
0 7
2 3 6 8 22 11 12
0 5
3 4 7 12 13
5 5
5 22 10 14 18
0 4
22 9 11 14
0 6
7 12 22 10 14 15
0 7
7 8 11 13 15 16 17
2 5
4 8 12 17 21
0 5
9 10 11 15 18
0 6
11 12 14 16 18 19
0 6
12 15 17 19 20 21
0 4
12 13 16 21
2 4
9 14 15 19
2 4
15 16 18 20
3 3
16 19 21
5 4
13 16 17 20
0 6
5 6 7 9 10 11
        ";
    }
}
