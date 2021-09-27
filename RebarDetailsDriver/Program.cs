using ACI318_19Library;
using System;

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
                ACI318_19Library.StraightDevelopmentLength dev_length = new ACI318_19Library.StraightDevelopmentLength(i, 60000, 3000, true);
                Console.WriteLine("#" + i + " bar - " + dev_length.DevLength().ToString() + " inches");
            }

            for (int i = 3; i <= 11; i++)
            {
                Console.WriteLine("------------------------------------------------");

                // 90 degree hook
                ACI318_19Library.HookDevelopmentLength dev_length = new ACI318_19Library.HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STANDARD, 90);
                dev_length.HookLength();

                // 135 degree hook
                dev_length = new HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STANDARD, 135);
                dev_length.HookLength();

                // 180 degree hook
                dev_length = new HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STANDARD, 180);
                dev_length.HookLength();

                Console.WriteLine("#" + i + " bar - " + dev_length.DevLength().ToString() + " inches");
            }

            for (int i = 3; i <= 11; i++)
            {
                Console.WriteLine("------------------------------------------------");

                // 90 degree hook
                HookDevelopmentLength dev_length = new HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STIRRUP_TIE, 90);
                dev_length.HookLength();

                // 135 degree hook
                dev_length = new HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STIRRUP_TIE, 135);
                dev_length.HookLength();

                // 180 degree hook
                dev_length = new HookDevelopmentLength(i, 60000, 3000, false, HookTypes.HOOK_STIRRUP_TIE, 180);
                dev_length.HookLength();

                Console.WriteLine("#" + i + " bar - " + dev_length.DevLength().ToString() + " inches");
            }



            Console.ReadLine();
        }
    }
}
