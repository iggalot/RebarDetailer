using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarDetailsLibrary
{
    public static class DevelopmentLength
    {
        const bool REBAR_DEBUG_MODE = true;
        public static List<string> StatusMessageList { get; set; }

        public static double Straight(
            out List<string> msgList,
            int bar_size,
            double steel_strength,
            double concrete_strength,
            bool DEBUG_MODE = !REBAR_DEBUG_MODE,
            double Ktr = 0,
            bool trans_reinf_provided = false,
            double adj_cc_spacing = 3,
            double side_cover = 3,
            double top_cover = 3,
            bool lightweight = false,
            bool epoxy_coated = true,
            bool top_bar_position = true
            )
        {
            string status_msg = "";
            double devLength = 0;
            double psi_e, psi_s, psi_t, psi_g, lambda, psi_c, psi_o;
            double psi_e_x_psi_t_product;
            double steel_yield_str, concrete_comp_str;
            bool spacing_and_cover_reqs_okay = false;

            StatusMessageList = new List<string>();

            if (bar_size < 3 || bar_size > 11)
                throw new InvalidOperationException("ERROR: Bar size must be between #3 and #11 - barSize #" + bar_size.ToString());

            if (steel_strength <= 0 || concrete_strength <= 0)
                throw new InvalidOperationException("ERROR: Material Strength is invalid - fy=" + steel_strength.ToString() + " f'c=" + concrete_strength);

            if (Ktr < 0)
                throw new InvalidOperationException("ERROR: Ktr cannot be less than zero - Ktr=" + Ktr.ToString());

            if (adj_cc_spacing < 0 || side_cover < 0 || top_cover < 0)
            {
                throw new InvalidOperationException("ERROR: Cover and spacing reqs must be positive = CC_spacing=" + adj_cc_spacing.ToString() + " : side_cover=" + side_cover.ToString() + " : top/bottom cover=" + top_cover.ToString());
            }

            double bar_dia = bar_size / 8.0;
            StatusMessageList.Add("Bar Size = " + bar_size + " -- dia. = " + bar_dia);

            steel_yield_str = steel_strength;
            concrete_comp_str = concrete_strength;
            StatusMessageList.Add("fy = " + steel_yield_str + "      f'c = " + concrete_comp_str);

            lambda = lightweight ? 0.75 : 1.0;
            StatusMessageList.Add("lambda = " + lambda.ToString() + " -- " + (lightweight ? "" : " NOT ") + "lightweight concrete");

            psi_e = ComputePSI_E(epoxy_coated, out status_msg);
            StatusMessageList.Add(status_msg);

            psi_s = ComputePSI_S(bar_size, out status_msg);
            StatusMessageList.Add(status_msg);

            psi_t = ComputePSI_T(top_bar_position, out status_msg);
            StatusMessageList.Add(status_msg);

            psi_g = ComputePSI_G(steel_yield_str, out status_msg);
            StatusMessageList.Add(status_msg);


            // Apply the limit from ACI318-19 Table 25.4.2.5 for product of psi_e and psi_t greater than 1.7
            psi_e_x_psi_t_product = ComputePSI_E_x_Psi_T_Product(psi_e, psi_t, out status_msg);
            StatusMessageList.Add(status_msg);

            // Compute cb from lesser of half of clear cc spacing or side or bottom covers
            double cb = ComputeCb(adj_cc_spacing, side_cover, top_cover, out status_msg);
            StatusMessageList.Add(status_msg);

            // Compute the (cb + Ktr) / db factor and apply limit of 2.5 from ACI318-19 25.4.2.4 
            double cb_plus_Ktr_factor = ComputeCb_Plus_Ktr(cb, Ktr, bar_dia, out status_msg);
            StatusMessageList.Add(status_msg);

            // Checks for ACI318-19 Table 25.4.2.3

            spacing_and_cover_reqs_okay = CheckSpacingAndCoverReqs(bar_dia, adj_cc_spacing, side_cover, top_cover, trans_reinf_provided, out status_msg);
            StatusMessageList.Add(status_msg);

            // Determine which equation from Table 25.4.2.3 to use for straight development length
            string devLengthEqString = "";
            if (bar_size <= 6)
            {
                devLengthEqString += "Bar size 6 and smaller";
                if (spacing_and_cover_reqs_okay)
                {
                    devLengthEqString += " & spacing reqs are okay (ACI318-19 Table 24.4.2.3).";
                    devLength = SmallBarDevLength_SpacingReqsOkay(bar_dia, steel_yield_str, concrete_comp_str, psi_e_x_psi_t_product, psi_g, lambda, DEBUG_MODE);
                } else
                {
                    devLengthEqString += " & spacing reqs exceed limits (ACI318-19 Table 24.4.2.3)";
                    devLength = SmallBarDevLength_SpacingReqsInvalid(bar_dia, steel_yield_str, concrete_comp_str, psi_e_x_psi_t_product, psi_g, lambda, DEBUG_MODE);
                }
            }
            else
            {
                devLengthEqString += "Bar size greater than 6";
                if (spacing_and_cover_reqs_okay)
                {
                    devLengthEqString += " & spacing reqs are okay (ACI318-19 Table 24.4.2.3).";
                    devLength = LargeBarDevLength_SpacingReqsOkay(bar_dia, steel_yield_str, concrete_comp_str, psi_e_x_psi_t_product, psi_g, lambda, DEBUG_MODE);
                }
                else
                {
                    devLengthEqString += " & spacing reqs exceed limits (ACI318-19 Table 24.4.2.3)";
                    devLength = LargeBarDevLength_SpacingReqsInvalid(bar_dia, steel_yield_str, concrete_comp_str, psi_e_x_psi_t_product, psi_g, lambda, DEBUG_MODE);
                }
            }
            StatusMessageList.Add(devLengthEqString);

            // Tension development length per ACI318-19 Eqn. 25.4.2.4a
            double devLength_allfactors = ((3 * steel_yield_str * psi_s * psi_g * psi_e_x_psi_t_product) / (40.0 * lambda * Math.Sqrt(concrete_comp_str) * cb_plus_Ktr_factor)) * bar_dia;

            if (DEBUG_MODE)
            {
                Console.WriteLine("   Analyzing a #" + bar_size.ToString() + " straight bar with fy=" + steel_yield_str.ToString() + " psi amd f'c=" + concrete_comp_str.ToString() + "psi");
                Console.WriteLine("   MATS: fy      f'c    lambda");
                Console.WriteLine("         " + steel_yield_str.ToString() + " : " + concrete_comp_str.ToString() + " : " + lambda.ToString());
                Console.WriteLine("   Epoxy coated: " + epoxy_coated.ToString() + " : Top bars: " + top_bar_position.ToString());

                string str = "   PSI  E     S     T     G";

                if ((psi_e * psi_t > 1.7))
                    str += " -- Table 25.4.2.5 limit of 1.7 triggered";
                Console.WriteLine(str);
                Console.WriteLine("        " + psi_e.ToString() + " : " + psi_s.ToString() + " : " + psi_t.ToString() + " : " + psi_g.ToString());

                if (Ktr == 0)
                    Console.WriteLine("   Conservative Ktr=0 used.");

                str += cb_plus_Ktr_factor.ToString();
                if ((cb + Ktr) / bar_dia >= 2.5)
                    str += ((cb + Ktr) / bar_dia).ToString() + " - but LIMIT TRIGGERED -- 2.5 used";

                Console.WriteLine("   Clear spacing= " + adj_cc_spacing.ToString() + " : side_cover= " + side_cover.ToString() + " : top_cover=" + top_cover.ToString());
                Console.WriteLine("   cb=" + cb.ToString() + " :   (Cb + Ktr) / db = " + str);

                Console.WriteLine(devLengthEqString);

                Console.WriteLine("    ld = " + devLength.ToString() + " vs. " + devLength_allfactors.ToString());
            }

            StatusMessageList.Add("Table 25.4.2.3 Simplified Eqns for LD = " + devLength.ToString());
            StatusMessageList.Add("ACI318-19 Eqn 25.4.2.4a for LD=" + devLength_allfactors.ToString());

            // return our messages to the calling function via the "out" variable
            msgList = StatusMessageList;

            // Round up the result and choose the smaller of the two computations
            return Math.Min(Math.Ceiling(devLength),Math.Ceiling(devLength_allfactors));
        }

        public static double ComputePSI_E(bool status, out string msg)
        {
            if (status)
            {
                msg = "PSI_E = 1.3 per ACI318-19 Table 25.4.2.5";
                return 1.3;
            }
            else
            {
                msg = "PSI_E = 1.0 per ACI318-19 Table 25.4.2.5";
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
            if (psi_e * psi_t > 1.7)
            {
                msg = "PSI_E x PSI_T product > 1.7. ACI318-19 Table 25.4.2.5 limits product to max of 1.7";
                return 1.7;
            } else
            {
                msg = "PSI_E x PSI_T product less than 1.7 -- ACI318-19 Table 25.4.2.5";
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
    }
}
