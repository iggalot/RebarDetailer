using RebarDetailsLibrary;
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
            Console.WriteLine(" Straight Bars");
            for (int i = 3; i <= 11; i++)
            {
                Console.WriteLine("------------------------------------------------");
                RebarDetailsLibrary.StraightDevelopmentLength dev_length = new RebarDetailsLibrary.StraightDevelopmentLength(i, 60000, 3000, true);
                Console.WriteLine("#" + i + " bar - " + dev_length.DevLength().ToString() + " inches");
            }

            for (int i = 3; i <= 11; i++)
            {
                Console.WriteLine("------------------------------------------------");
                
                // 90 degree hook
                RebarDetailsLibrary.HookDevelopmentLength dev_length = new RebarDetailsLibrary.HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STANDARD, 90);
                dev_length.HookLength();

                // 135 degree hook
                dev_length = new RebarDetailsLibrary.HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STANDARD, 135);
                dev_length.HookLength();

                // 180 degree hook
                dev_length = new RebarDetailsLibrary.HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STANDARD, 180);
                dev_length.HookLength();

                Console.WriteLine("#" + i + " bar - " + dev_length.DevLength().ToString() + " inches");
            }

            for (int i = 3; i <= 11; i++)
            {
                Console.WriteLine("------------------------------------------------");

                // 90 degree hook
                RebarDetailsLibrary.HookDevelopmentLength dev_length = new RebarDetailsLibrary.HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STIRRUP_TIE, 90);
                dev_length.HookLength();

                // 135 degree hook
                dev_length = new RebarDetailsLibrary.HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STIRRUP_TIE, 135);
                dev_length.HookLength();

                // 180 degree hook
                dev_length = new RebarDetailsLibrary.HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STIRRUP_TIE, 180);
                dev_length.HookLength();

                Console.WriteLine("#" + i + " bar - " + dev_length.DevLength().ToString() + " inches");
            }



            Console.ReadLine();
        }
    }
}
