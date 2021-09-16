using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarDetailsDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Rebar detail driver v1.0");

            for (int i = 3; i < 11; i++)
            {
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine("#" + i + " bar - " + RebarDetailsLibrary.DevelopmentLength.Straight(i, 60000, 3000, false).ToString() + " inches");
            }

            Console.ReadLine();
        }
    }
}
