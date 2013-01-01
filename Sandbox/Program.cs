using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Advent.Common.Diagnostics;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            TraceUtil.SetupFileTrace(@"c:\tmp\cow.txt");
        }
    }
}
