using System;

namespace RebarDetailsLibrary
{
    public class HookDevelopmentLength : BaseDevelopmentLength
    {
        public double PSI_E { get; set; }
        public double PSI_R { get; set; }
        public double PSI_O { get; set; }
        public double PSI_C { get; set; }

        public double A_TH {get; set; } = 0.0;
        public double A_HS { get; set; } = 0.0;

        public bool TerminateInsideColumnStatus { get; set; } = false;



        public HookDevelopmentLength(
            int bar_size,
            double steel_strength,
            double concrete_strength,
            bool DEBUG_MODE,
            double ktr = 0,
            double adj_cc_spacing = 3,
            double side_cover = 3,
            double bottom_cover = 3,
            bool lightweight = false,
            bool epoxy_coated = true,
            bool terminate_in_column = false
            ) : base(
                DevelopmentLengthTypes.DEV_LENGTH_HOOKED,
                bar_size,
                steel_strength,
                concrete_strength,
                DEBUG_MODE,
                adj_cc_spacing,
                side_cover,
                bottom_cover,
                lightweight,
                epoxy_coated)
        { 

        }

        public double HookLength()
        {

            string status_msg = "";

            PSI_E = ComputePSI_E(EpoxyBarStatus, out status_msg);
            StatusMessageList.Add(status_msg);


            PSI_R = ComputePSI_R(BarSize, BarDiameter, A_TH, A_HS, CC_Spacing, out status_msg);
            StatusMessageList.Add(status_msg);


            PSI_O = ComputePSI_O(TerminateInsideColumnStatus, BarSize, BarDiameter, out status_msg);
            StatusMessageList.Add(status_msg);

            PSI_C = ComputePSI_C(ConcreteCompStrength, out status_msg);
            StatusMessageList.Add(status_msg);

            if (base.DEBUG_MODE)
            {
                Console.WriteLine("   Analyzing a #" + BarSize.ToString() + " straight bar with fy=" + SteelYieldStrength.ToString() + " psi amd f'c=" + ConcreteCompStrength.ToString() + "psi");
                Console.WriteLine("   MATS: fy      f'c    lambda");
                Console.WriteLine("         " + SteelYieldStrength.ToString() + " : " + ConcreteCompStrength.ToString() + " : " + Lambda.ToString());
                Console.WriteLine("   Epoxy coated: " + EpoxyBarStatus.ToString() + " : Col. termination: " + TerminateInsideColumnStatus.ToString());

                string str = "   PSI  E     R     O     C";

                Console.WriteLine("        " + PSI_E.ToString() + " : " + PSI_R.ToString() + " : " + PSI_O.ToString() + " : " + PSI_C.ToString());

                //if (K_TR == 0)
                //    Console.WriteLine("   Conservative Ktr=0 used.");

                //str += cb_plus_Ktr_factor.ToString();
                //if ((cb + K_TR) / BarDiameter >= 2.5)
                //    str += ((cb + K_TR) / BarDiameter).ToString() + " - but LIMIT TRIGGERED -- 2.5 used";

                Console.WriteLine("   Clear spacing= " + CC_Spacing.ToString() + " : side_cover= " + SideCover.ToString() + " : top_cover=" + BottomCover.ToString());
                //Console.WriteLine("   cb=" + cb.ToString() + " :   (Cb + Ktr) / db = " + str);

                //Console.WriteLine(devLengthEqString);

                //Console.WriteLine("    ld = " + devLength.ToString() + " vs. " + devLength_allfactors.ToString());
            }

            return -1;
        }

        public static double ComputePSI_E(bool status, out string msg)
        {
            if (status)
            {
                msg = "PSI_E = 1.2 for epoxy coating per ACI318-19 Table 25.4.3.2";
                return 1.2;
            } else {
                msg = "PSI_E = 1.0 for normal coating per ACI318-19 Table 25.4.3.2";
                return 1.0;
            }
        }

        public static double ComputePSI_R(int bar_size, double bar_dia, double A_th, double A_hs, double s, out string msg)
        {
            if (bar_size <= 11)
            {
                if((A_th >= 0.4) || (s >= 6 * bar_dia)){
                    msg = "PSI_R - Meets requirements of Table 25.4.3.2";
                    return 1.0;
                }
            }

            msg = "PSI_R - Does not meet requirements of Table 25.4.3.2";
            return 1.6;
        }

        public double ComputePSI_O(bool col_status, int bar_size, double bar_dia, out string msg)
        {
            if (bar_dia <= 11)
            {
                if (col_status && SideCover >= 2.5){
                    msg = "PSI_O - Bar size is 11 or smaller and terminates in column with more than 2.5 in. side cover";
                    return 1.0;
                } else if (SideCover > 6 * bar_dia)
                {
                    msg = "PSI_O - Bar size is 11 and side cover > 6*db and does not terminate in column";
                    return 1.0;
                }
            }
            msg = "Does not meet requirements of Table 25.4.3.2";
            return 1.25;
        }

        public static double ComputePSI_C(double fc, out string msg)
        {
            if (fc < 6000)
            {
                double val = fc / 15000 + 0.6;
                msg = "PSI_C = " + val.ToString() + " per ACI318-19 Table 25.4.2.5";
                return val;
            }
            else
            {
                msg = "PSI_C = 1.0 for f'c < 6000psi per ACI318-19 Table 25.4.2.5";
                return 1.0;
            }
        }


    }
}
