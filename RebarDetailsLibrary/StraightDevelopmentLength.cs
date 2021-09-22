using System;
using System.Linq;
using System.Windows.Controls;

namespace RebarDetailsLibrary
{
    public class StraightDevelopmentLength : BaseDevelopmentLength
    {
        public bool TopBarStatus { get; set; } = true;
        public bool HasMinTransverseReinf { get; set; } = false;
        public double K_TR { get; set; } = 0;
        public double DevelopmentLengthStraight { get; set; } = 0;
        public bool TransReinfProvided { get; set; } = false;


        public double PSI_E { get; set; }
        public double PSI_G { get; set; }
        public double PSI_T { get; set; }
        public double PSI_S { get; set; }


        public StraightDevelopmentLength(
            int bar_size,
            double steel_strength,
            double concrete_strength,
            bool DEBUG_MODE,
            double ktr = 0,
            bool tr_provided = false,
            double adj_cc_spacing = 3,
            double side_cover = 3,
            double bottom_cover = 3,
            bool lightweight = false,
            bool epoxy_coated = true,
            bool top_bar_position = true
            ) : base(
                DevelopmentLengthTypes.DEV_LENGTH_STRAIGHT,
                bar_size,
                steel_strength,
                concrete_strength,
                DEBUG_MODE,
                adj_cc_spacing,
                side_cover,
                bottom_cover,
                lightweight ,
                epoxy_coated)
        {

            if (ktr < 0)
                throw new InvalidOperationException("ERROR: Ktr cannot be less than zero - Ktr=" + ktr.ToString());

            K_TR = ktr;
            HasMinTransverseReinf = tr_provided;
            TopBarStatus = top_bar_position;
        }

        public double Straight()
        {
            string status_msg = "";
            double psi_e_x_psi_t_product;
            bool spacing_and_cover_reqs_okay = false;

//            StatusMessageList = new List<string>();

            PSI_E = ComputePSI_E(EpoxyBarStatus, BarDiameter, CC_Spacing, SideCover, BottomCover,  out status_msg);
            StatusMessageList.Add(status_msg);

            PSI_S = ComputePSI_S(BarSize, out status_msg);
            StatusMessageList.Add(status_msg);

            PSI_T = ComputePSI_T(TopBarStatus, out status_msg);
            StatusMessageList.Add(status_msg);

            PSI_G = ComputePSI_G(SteelYieldStrength, out status_msg);
            StatusMessageList.Add(status_msg);

            // Apply the limit from ACI318-19 Table 25.4.2.5 for product of psi_e and psi_t greater than 1.7
            psi_e_x_psi_t_product = ComputePSI_E_x_Psi_T_Product(PSI_E, PSI_T, out status_msg);
            StatusMessageList.Add(status_msg);

            // Compute cb from lesser of half of clear cc spacing or side or bottom covers
            double cb = ComputeCb(CC_Spacing, SideCover, BottomCover, out status_msg);
            StatusMessageList.Add(status_msg);

            // Compute the (cb + Ktr) / db factor and apply limit of 2.5 from ACI318-19 25.4.2.4 
            double cb_plus_Ktr_factor = ComputeCb_Plus_Ktr(cb, K_TR, BarDiameter, out status_msg);
            StatusMessageList.Add(status_msg);

            // Checks for ACI318-19 Table 25.4.2.3
            spacing_and_cover_reqs_okay = CheckSpacingAndCoverReqs(BarDiameter, CC_Spacing, SideCover, BottomCover, TransReinfProvided, out status_msg);
            StatusMessageList.Add(status_msg);

            // Determine which equation from Table 25.4.2.3 to use for straight development length
            string devLengthEqString = "";
            double devLength;
            if (BarSize <= 6)
            {
                devLengthEqString += "Bar size 6 and smaller";
                if (spacing_and_cover_reqs_okay)
                {
                    devLengthEqString += " & spacing reqs are okay (ACI318-19 Table 24.4.2.3).";
                    devLength = SmallBarDevLength_SpacingReqsOkay(BarDiameter, SteelYieldStrength, ConcreteCompStrength, psi_e_x_psi_t_product, PSI_G, Lambda, DEBUG_MODE);
                } else
                {
                    devLengthEqString += " & spacing reqs exceed limits (ACI318-19 Table 24.4.2.3)";
                    devLength = SmallBarDevLength_SpacingReqsInvalid(BarDiameter, SteelYieldStrength, ConcreteCompStrength, psi_e_x_psi_t_product, PSI_G, Lambda, DEBUG_MODE);
                }
            }
            else
            {
                devLengthEqString += "Bar size greater than 6";
                if (spacing_and_cover_reqs_okay)
                {
                    devLengthEqString += " & spacing reqs are okay (ACI318-19 Table 24.4.2.3).";
                    devLength = LargeBarDevLength_SpacingReqsOkay(BarDiameter, SteelYieldStrength, ConcreteCompStrength, psi_e_x_psi_t_product, PSI_G, Lambda, DEBUG_MODE);
                }
                else
                {
                    devLengthEqString += " & spacing reqs exceed limits (ACI318-19 Table 24.4.2.3)";
                    devLength = LargeBarDevLength_SpacingReqsInvalid(BarDiameter, SteelYieldStrength, ConcreteCompStrength, psi_e_x_psi_t_product, PSI_G, Lambda, DEBUG_MODE);
                }
            }
            StatusMessageList.Add(devLengthEqString);

            // Tension development length per ACI318-19 Eqn. 25.4.2.4a
            double devLength_allfactors = (3 * SteelYieldStrength * PSI_S * PSI_G * psi_e_x_psi_t_product / (40.0 * Lambda * Math.Sqrt(ConcreteCompStrength) * cb_plus_Ktr_factor)) * BarDiameter;

            if (devLength_allfactors < 12)
            {
                StatusMessageList.Add("- min. LD from 25.4.2.1(b) = 12 in. controls.");
            }
            else
            {
                StatusMessageList.Add("- LD calculated from 25.4.2.1(a) controls.");
            }

            devLength_allfactors = Math.Max(devLength_allfactors, 12.0);

            if (DEBUG_MODE)
            {
                Console.WriteLine("   Analyzing a #" + BarSize.ToString() + " straight bar with fy=" + SteelYieldStrength.ToString() + " psi amd f'c=" + ConcreteCompStrength.ToString() + "psi");
                Console.WriteLine("   MATS: fy      f'c    lambda");
                Console.WriteLine("         " + SteelYieldStrength.ToString() + " : " + ConcreteCompStrength.ToString() + " : " + Lambda.ToString());
                Console.WriteLine("   Epoxy coated: " + EpoxyBarStatus.ToString() + " : Top bars: " + TopBarStatus.ToString());

                string str = "   PSI  E     S     T     G";

                if ((PSI_E * PSI_T > 1.7))
                    str += " -- Table 25.4.2.5 limit of 1.7 triggered";
                Console.WriteLine(str);
                Console.WriteLine("        " + PSI_E.ToString() + " : " + PSI_S.ToString() + " : " + PSI_T.ToString() + " : " + PSI_G.ToString());

                if (K_TR == 0)
                    Console.WriteLine("   Conservative Ktr=0 used.");

                str += cb_plus_Ktr_factor.ToString();
                if ((cb + K_TR) / BarDiameter >= 2.5)
                    str += ((cb + K_TR) / BarDiameter).ToString() + " - but LIMIT TRIGGERED -- 2.5 used";

                Console.WriteLine("   Clear spacing= " + CC_Spacing.ToString() + " : side_cover= " + SideCover.ToString() + " : top_cover=" + BottomCover.ToString());
                Console.WriteLine("   cb=" + cb.ToString() + " :   (Cb + Ktr) / db = " + str);

                Console.WriteLine(devLengthEqString);

                Console.WriteLine("    ld = " + devLength.ToString() + " vs. " + devLength_allfactors.ToString());
            }

            StatusMessageList.Add("Table 25.4.2.3 Simplified Eqns for LD = " + devLength.ToString());
            StatusMessageList.Add("ACI318-19 Eqn 25.4.2.4a for LD=" + devLength_allfactors.ToString());

            // Round up the result and choose the smaller of the two computations
            return Math.Min(Math.Ceiling(devLength),Math.Ceiling(devLength_allfactors));
        }

        public static double ComputePSI_E(bool status, double bar_dia, double clear_spacing, double side_cover, double bottom_cover, out string msg)
        {
            double smallestCover = Math.Min(side_cover, bottom_cover);

            if (status)
            {
                // clear spacing less than 6 * db or clear cover less than 3 * db
                if ((smallestCover < 6 * bar_dia) || (clear_spacing < 3 * bar_dia))
                {
                    
                    msg = "PSI_E = 1.5 per ACI318-19 Table 25.4.2.5\nEpoxy coated and cover less than 3db or clear spacing less than 6db";
                    return 1.5;
                } else
                {
                    msg = "PSI_E = 1.2 per ACI318-19 Table 25.4.2.5\nEpoxy coated with no cover or spacing issues";
                    return 1.2;
                }
            }
            else
            {
                msg = "PSI_E = 1.0 per ACI318-19 Table 25.4.2.5\nNon-epoxy coated";
                return 1.0;
            }
        }

        public static double ComputePSI_G(double yield, out string msg)
        {
            if (yield <= 60000)
            {
                msg = "PSI_G = 1.0 per ACI318-19 Table 25.4.2.5";
                return 1.0;
            }
            else if (yield > 80000)
            {
                msg = "PSI_G = 1.3 per ACI318-19 Table 25.4.2.5";
                return 1.3;
            }
            else
            {
                msg = "PSI_G = 1.15 per ACI318-19 Table 25.4.2.5";
                return 1.15;
            }
        }

        public static double ComputePSI_S(int size, out string msg)
        {
            if (size <= 7)
            {
                msg = "PSI_S = 0.7 per ACI318-19 Table 25.4.2.5";
                return 0.7;
            }
            else
            {
                msg = "PSI_G = 1.0 per ACI318-19 Table 25.4.2.5";
                return 1.0;
            }
        }

        public static double ComputePSI_T(bool status, out string msg)
        {
            if (status)
            {
                msg = "PSI_T = 1.2 per ACI318-19 Table 25.4.2.5";
                return 1.2;
            }
            else
            {
                msg = "PSI_T = 1.0 per ACI318-19 Table 25.4.2.5";
                return 1.0;
            }
        }

        private static double ComputePSI_E_x_Psi_T_Product(double psi_e, double psi_t, out string msg)
        {
            msg = "PSI_E x PSI_T product ";
            if (psi_e * psi_t > 1.7)
            {
                msg += " is limited at max of 1.7 (computed as " + (psi_e * psi_t).ToString() + ") per ACI318-19 Table 25.4.2.5";
                return 1.7;
            } else
            {
                msg += "= " + (psi_e * psi_t).ToString() + "is less than 1.7 so no limit imposed per ACI318-19 Table 25.4.2.5";
                return (psi_e * psi_t);
            }

        }

        private static double ComputeCb(double adj_cc_spacing, double side_cover, double top_cover, out string msg)
        {
            double cb = (new double[] { adj_cc_spacing / 2.0, side_cover, top_cover }).Min();
            msg = "cb = " + cb.ToString() + " -- ";
            if (cb == adj_cc_spacing / 2.0)
            {
                msg += "clear spacing / 2.0 controls : ";
            }
            if (cb == side_cover)
            {
                msg += "side cover controls : ";
            }
            if(cb == top_cover)
            {
                msg += "top cover controls";
            }

            return cb;
        }
        public static double ComputeCb_Plus_Ktr(double cb, double ktr, double bar_dia, out string msg)
        {
            if (((cb + ktr) / bar_dia) > 2.5)
            {
                msg = "(cb + Ktr) / db factor exceeds limit of 2.5 from ACI318-19 25.4.2.4";
                return 2.5;
            } else
            {
                msg = "(cb + Ktr) / db limit is less than 2.5 from ACI318-19 25.4.2.4";
                return (cb + ktr) / bar_dia;
            }
        }
        private static bool CheckSpacingAndCoverReqs(double bar_dia, double adj_cc_spacing, double side_cover, double top_cover, bool trans_reinf_provided, out string msg)
        {
            bool spacing_check = adj_cc_spacing > bar_dia ? true : false;
            bool cover_check = ((side_cover > bar_dia) && (top_cover > bar_dia)) ? true : false;
            bool trans_stirrups_throughout_check = trans_reinf_provided;

            if(spacing_check)
            {
                msg = "ACI318-19 Table 25.4.2.3 spacing reqs okay\n";
            } else
            {
                msg = "ACI318-19 Table 25.4.2.3 spacing reqs not okay\n";
            }

            if(cover_check)
            {
                msg += "ACI318-19 Table 25.4.2.3 top and side cover reqs okay\n";
            }
            else
            {
                msg += "ACI318-19 Table 25.4.2.3 top / bot and side cover reqs not okay\n";
            }

            if(trans_stirrups_throughout_check)
            {
                msg += "ACI318-19 Table 25.4.2.3 transverse reinforcing reqs okay\n";
            }
            else
            {
                msg += "ACI318-19 Table 25.4.2.3 transverse reinforcing reqs not okay\n";
            }

            if (spacing_check && cover_check && trans_stirrups_throughout_check)
                msg += "All Table 25.4.2.3 spacing checks OKAY";

            return (spacing_check && cover_check && trans_stirrups_throughout_check);
        }

        #region Table 25.4.2.3 equations
        private static double SmallBarDevLength_SpacingReqsOkay(double dia, double fy, double fc, double psi_e_x_psi_t_product, double psi_g, double lambda, bool mode)
        {
            return Math.Max(12, (fy * psi_e_x_psi_t_product * psi_g) / (25.0 * lambda * Math.Sqrt(fc)) * dia);
        }

        private static double SmallBarDevLength_SpacingReqsInvalid(double dia, double fy, double fc, double psi_e_x_psi_t_product, double psi_g, double lambda, bool mode)
        {
            return Math.Max(12, (3.0 * fy * psi_e_x_psi_t_product * psi_g) / (50.0 * lambda * Math.Sqrt(fc)) * dia);
        }

        private static double LargeBarDevLength_SpacingReqsOkay(double dia, double fy, double fc, double psi_e_x_psi_t_product, double psi_g, double lambda, bool mode)
        {
            return Math.Max(12, (fy * psi_e_x_psi_t_product * psi_g) / (20.0 * lambda * Math.Sqrt(fc)) * dia);
        }

        private static double LargeBarDevLength_SpacingReqsInvalid(double dia, double fy, double fc, double psi_e_x_psi_t_product, double psi_g, double lambda, bool mode)
        {
            return Math.Max(12, (3.0 * fy * psi_e_x_psi_t_product * psi_g) / (40.0 * lambda * Math.Sqrt(fc)) * dia);
        }
        #endregion

        public override double DevLength()
        {
            double val = Straight();
            return val;
        }
    }
}
