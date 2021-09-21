using System;
using System.Linq;

namespace RebarDetailsLibrary
{
    public enum HookTypes
    {
        HOOK_UNDEFINED = -1,
        HOOK_STANDARD = 0,
        HOOK_STIRRUP_TIE = 1
    }

    public class HookDevelopmentLength : BaseDevelopmentLength
    {
        public double PSI_E { get; set; }
        public double PSI_R { get; set; }
        public double PSI_O { get; set; }
        public double PSI_C { get; set; }

        // Total cross sectional area of bars confining hook
        public double A_TH {get; set; } = 0.0;

        // Total cross sectional area of hooks or headed bars being developed at critical section
        public double A_HS { get; set; } // don't make this zero...it breaks the PSI_R checks

        public double LDH { get; set; } = -1;   // development length of standard hook (inlcudes bend diameter)
        public double BendDia { get; set; } = 0; // inside bend diameter of hook
        public double L_EXT { get; set; } = -1; // extension after bend diemater
        public double BendAngle { get; set; } = 90; // Angle of the bend in degrees

        public HookTypes HookType { get; set; } = HookTypes.HOOK_UNDEFINED;
        public bool TerminateInsideColumnStatus { get; set; } = false;

        public HookDevelopmentLength(
            int bar_size,
            double steel_strength,
            double concrete_strength,
            bool DEBUG_MODE,
            HookTypes hook_type,
            double bend_angle,
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
            HookType = hook_type;
            BendAngle = bend_angle;
            A_HS = (Math.PI * BarDiameter * BarDiameter / 4.0);
        }

        public double HookLength()
        {
            string status_msg = "";

            StatusMessageList.Add("   HookType: " + HookType + "  BendAngle: " + BendAngle.ToString());

            PSI_E = ComputePSI_E(EpoxyBarStatus, out status_msg);
            StatusMessageList.Add(status_msg);


            PSI_R = ComputePSI_R(out status_msg);
            StatusMessageList.Add(status_msg);


            PSI_O = ComputePSI_O(TerminateInsideColumnStatus, out status_msg);
            StatusMessageList.Add(status_msg);

            PSI_C = ComputePSI_C(out status_msg);
            StatusMessageList.Add(status_msg);

            string devLengthEqString = "";
            DetermineHookLengthValues();
            StatusMessageList.Add("LDH: " + LDH.ToString() + "  L_EXT: " + L_EXT.ToString() + "   BendDia: " + BendDia.ToString() + "  BendAngle: " + BendAngle.ToString());
            


            if (base.DEBUG_MODE)
            {
                Console.WriteLine("   Analyzing a #" + BarSize.ToString() + " hooked bar with fy=" + SteelYieldStrength.ToString() + " psi amd f'c=" + ConcreteCompStrength.ToString() + "psi");
                Console.WriteLine("   HookType: " + HookType + " -- 0=Standard, 1=Stirrup/Tie");
                Console.WriteLine("   MATS: fy      f'c    lambda");
                Console.WriteLine("         " + SteelYieldStrength.ToString() + " : " + ConcreteCompStrength.ToString() + " : " + Lambda.ToString());
                Console.WriteLine("   Epoxy coated: " + EpoxyBarStatus.ToString() + " : Col. termination: " + TerminateInsideColumnStatus.ToString());

                string str = "   PSI  E     R     O     C";
                Console.WriteLine("        " + PSI_E.ToString() + " : " + PSI_R.ToString() + " : " + PSI_O.ToString() + " : " + PSI_C.ToString());
                Console.WriteLine("    Bar terminates in column? " + TerminateInsideColumnStatus);
                Console.WriteLine("   Clear spacing= " + CC_Spacing.ToString() + " : side_cover= " + SideCover.ToString() + " : top_cover=" + BottomCover.ToString());
            }

            string status_string = "";
            foreach(var item in StatusMessageList)
            {
                status_string += item + "\n";
            }
            Console.WriteLine(status_string);

            return Math.Ceiling(LDH);
        }
        private void DetermineHookLengthValues()
        {
            string status_msg = "";
            LDH = ComputeLDH(out status_msg);
            StatusMessageList.Add("LDH: " + LDH.ToString() + " " + status_msg);

            L_EXT = ComputeL_EXT(out status_msg);
            StatusMessageList.Add("L_EXT: " + L_EXT.ToString() + " " + status_msg);

            BendDia = ComputeBendDia(out status_msg);
            StatusMessageList.Add("L_BendDiameter = " + BendDia + " " + status_msg);
        }

        private double ComputeL_EXT(out string msg)
        {
            double bend_dia;
            switch (HookType)
            {
                case HookTypes.HOOK_STANDARD:
                    bend_dia = ComputeL_EXT_ForStandardHook(out msg);
                    break;
                case HookTypes.HOOK_STIRRUP_TIE:
                    bend_dia = ComputeL_EXT_ForStirrupsTies(out msg);
                    break;
                case HookTypes.HOOK_UNDEFINED:
                default:
                    bend_dia = -1;
                    msg = "Invalid hook type received: " + HookType.ToString();
                    break;
            }

            return bend_dia;
        }

        protected double ComputeBendDia(out string msg)
        {
            double bend_dia;
            switch (HookType)
            {
                case HookTypes.HOOK_STANDARD:
                    bend_dia = ComputeBendDia_ForStandardHook(out msg);
                    break;
                case HookTypes.HOOK_STIRRUP_TIE:
                    bend_dia = ComputeBendDia_ForStirrupsTies(out msg);
                    break;
                case HookTypes.HOOK_UNDEFINED:
                default:
                    bend_dia = -1;
                    msg = "Invalid hook type received: " + HookType.ToString();
                    break;
            }

            return bend_dia;
        }

        private double ComputeBendDia_ForStandardHook(out string msg)
        {
            msg = BendAngle.ToString() + " deg hook (Table 25.3.1) ";

            if (BendAngle == 90)
            {
                if (BarSize <= 8)
                {
                    msg += " for bar size #3, 4, 5, 6, 7, 8";
                    return 6 * BarDiameter;
                }
                else if (BarSize == 9 || BarSize == 10 || BarSize == 11)
                {
                    msg += " for bar size #9, 10, or 11";
                    return 8 * BarDiameter;
                }
                else
                {
                    msg += " for bar size 14 and 18";
                    return 10 * BarDiameter;
                }
            }
            else if (BendAngle == 135)
            {
                    msg += " is non standard";
                    return -1;
            }
            else if (BendAngle == 180)
            {
                if (BarSize <= 8)
                {
                    msg += " for bar size #3, 4, 5, 6, 7, 8";
                    return 6 * BarDiameter;
                }
                else if (BarSize == 9 || BarSize == 10 || BarSize == 11)
                {
                    msg += " for bar size #9, 10, or 11";
                    return 8 * BarDiameter;
                }
                else
                {
                    msg += " for bar size 14 and 18";
                    return 10 * BarDiameter;
                }
            }
            else
            {
                msg += " is non standard for angles other than 90, and 180";
                return -1;
            }
        }

        private double ComputeBendDia_ForStirrupsTies(out string msg)
        {
            msg = BendAngle.ToString() + " deg hook (Table 25.3.2) ";

            if (BendAngle == 90)
            {
                if (BarSize == 3 || BarSize == 4 || BarSize == 5)
                {
                    msg += " for bar size #3, 4, or 5";
                    return 4 * BarDiameter;
                }
                else if (BarSize == 6 || BarSize == 7 || BarSize == 8)
                {
                    msg += " for bar size #6, 7, or 8";
                    return 6 * BarDiameter;
                }
                else
                {
                    msg += " is non standard for sizes greater than 9";
                    return -1;
                }
            }
            else if (BendAngle == 135)
            {
                if (BarSize == 3 || BarSize == 4 || BarSize == 5)
                {
                    msg += " for bar size #3, 4, or 5";
                    return 4 * BarDiameter;
                }
                else if (BarSize == 6 || BarSize == 7 || BarSize == 8)
                {
                    msg += " for bar size #6, 7, or 8";
                    return 6 * BarDiameter;
                }
                else
                {
                    msg += " is non standard for sizes greater than 9";
                    return -1;
                }
            }
            else if (BendAngle == 180)
            {
                if (BarSize == 3 || BarSize == 4 || BarSize == 5)
                {
                    msg += " for bar size #3, 4, or 5";
                    return 4 * BarDiameter;
                }
                else if (BarSize == 6 || BarSize == 7 || BarSize == 8)
                {
                    msg += " for bar size #6, 7, or 8";
                    return 6 * BarDiameter;
                }
                else
                {
                    msg += " is non standard for sizes greater than 9";
                    return -1;
                }
            }
            else
            {
                msg += " is non standard for angles other than 90, 135, and 180";
                return -1;
            }
        }

        private double ComputeL_EXT_ForStandardHook(out string msg)
        {
            msg = BendAngle.ToString() + " deg hook (Table 25.3.1) ";

            if (BendAngle == 90)
            {
                if (BarSize <= 8)
                {
                    msg += " for bar size #3, 4, 5, 6, 7, 8";
                    return 12 * BarDiameter;
                }
                else if (BarSize == 9 || BarSize == 10 || BarSize == 11)
                {
                    msg += " for bar size #9, 10, or 11";
                    return 12 * BarDiameter;
                }
                else
                {
                    msg += " for bar size 14 and 18";
                    return 12 * BarDiameter;
                }
            }
            else if (BendAngle == 135)
            {
                msg += " is non standard";
                return -1;
            }
            else if (BendAngle == 180)
            {
                if (BarSize <= 8)
                {
                    msg += " for bar size #3, 4, 5, 6, 7, 8";
                    return Math.Max(4 * BarDiameter, 2.5);
                }
                else if (BarSize == 9 || BarSize == 10 || BarSize == 11)
                {
                    msg += " for bar size #9, 10, or 11";
                    return Math.Max(4 * BarDiameter, 2.5);
                }
                else
                {
                    msg += " for bar size 14 and 18";
                    return Math.Max(4 * BarDiameter, 2.5);
                }
            }
            else
            {
                msg += " is non standard for angles other than 90, and 180";
                return -1;
            }
        }

        private double ComputeL_EXT_ForStirrupsTies(out string msg)
        {
            msg = BendAngle.ToString() + " deg hook (Table 25.3.2) ";

            if (BendAngle == 90)
            {
                if (BarSize == 3 || BarSize == 4 || BarSize == 5) {
                    msg += " for bar size #3, 4, or 5";
                    return Math.Max(6 * BarDiameter, 3);
                } else if (BarSize == 6 || BarSize == 7 || BarSize == 8)
                {
                    msg += " for bar size #6, 7, or 8";
                    return 12 * BarDiameter;
                } else
                {
                    msg += " is non standard for sizes greater than 9";
                    return -1;
                }
            } else if (BendAngle == 135)
            {
                if (BarSize == 3 || BarSize == 4 || BarSize == 5)
                {
                    msg += " for bar size #3, 4, or 5";
                    return Math.Max(6 * BarDiameter, 3);
                }
                else if (BarSize == 6 || BarSize == 7 || BarSize == 8)
                {
                    msg += " for bar size #6, 7, or 8";
                    return Math.Max(6 * BarDiameter, 3);
                }
                else
                {
                    msg += " is non standard for sizes greater than 9";
                    return -1;
                }
            } else if (BendAngle == 180)
            {
                if (BarSize == 3 || BarSize == 4 || BarSize == 5)
                {
                    msg += " for bar size #3, 4, or 5";
                    return Math.Max(4 * BarDiameter, 2.5);
                }
                else if (BarSize == 6 || BarSize == 7 || BarSize == 8)
                {
                    msg += " for bar size #6, 7, or 8";
                    return Math.Max(4 * BarDiameter, 2.5);
                }
                else
                {
                    msg += " is non standard for sizes greater than 9";
                    return -1;
                }
            } else
            {
                msg += " is non standard for angles other than 90, 135, and 180";
                return -1;
            }
        }

        protected double ComputeLDH(out string msg)
        {
            double ldh_calc = (SteelYieldStrength * PSI_E * PSI_R * PSI_O * PSI_C) / (55.0 * Lambda * Math.Sqrt(ConcreteCompStrength)) * Math.Pow(BarDiameter, 1.5);
            double ldh_min1 = 8 * BarDiameter;
            double ldh_min2 = 6.0;

            // Determine largest value
            double ldh = (new double[] { ldh_calc, ldh_min1, ldh_min2 }).Max();

            if (ldh == ldh_calc)
                msg = " computed from eqn 25.4.3.1(a)";
            else if (ldh == ldh_min1) 
            {
                msg = " computed from 25.4.3.1(b) -- (8 * db)";
            }
            else 
                // ldh equals ldh_min2
                msg = "- computed from25.4.3.1(c) -- (6 in.)";
            return ldh;
        }

        public double ComputePSI_E(bool status, out string msg)
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

        public double ComputePSI_R(out string msg)
        {
            if (BarSize <= 11)
            {
                if((A_TH >= 0.4 * A_HS) || (CC_Spacing >= 6 * BarDiameter)){
                    msg = "PSI_R = 1.0 - Meets requirements of Table 25.4.3.2";
                    return 1.0;
                }
            }

            msg = "PSI_R = 1.6 - Does not meet requirements of Table 25.4.3.2";
            return 1.6;
        }

        public double ComputePSI_O(bool col_status, out string msg)
        {
            if (BarSize <= 11)
            {
                if (col_status && SideCover >= 2.5){
                    msg = "PSI_O = 1.0 - Bar size is 11 or smaller and terminates in column with more than 2.5 in. side cover";
                    return 1.0;
                } else if (SideCover >= 6 * BarDiameter)
                {
                    msg = "PSI_O = 1.0 - Bar size is 11 or smaller and side cover > 6*db and does not terminate in column";
                    return 1.0;
                }
            }
            msg = "PSI_O = 1.25 - Does not meet requirements of Table 25.4.3.2";
            return 1.25;
        }

        public double ComputePSI_C(out string msg)
        {
            if (ConcreteCompStrength < 6000)
            {
                double val = (ConcreteCompStrength / 15000) + 0.6;
                msg = "PSI_C = " + val.ToString() + " per ACI318-19 Table 25.4.2.5";
                return val;
            }
            else
            {
                msg = "PSI_C = 1.0 for f'c < 6000psi per ACI318-19 Table 25.4.2.5";
                return 1.0;
            }
        }

        public override double DevLength()
        {
            double val = HookLength();
            return val;
        }
    }
}
