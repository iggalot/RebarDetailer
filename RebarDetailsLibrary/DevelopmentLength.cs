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

        public static double Straight(
            int bar_size, 
            double steel_strength, 
            float concrete_strength,
            bool DEBUG_MODE = !REBAR_DEBUG_MODE,
            double Ktr = 0, 
            bool trans_reinf_provided = false, 
            double adj_cc_spacing = 3, 
            double side_cover = 3, 
            double top_cover = 3, 
            bool lightweight = false, 
            bool epoxy_coated = true, 
            bool top_bar_position = true)
        {
            double devLength = 0;
            double psi_e, psi_s, psi_t, psi_g, lambda, psi_c, psi_o;
            double psi_e_x_psi_t_product;
            double steel_yield_str, concrete_comp_str;
            bool spacing_check, clear_check, trans_stirrups_throughout_check;
            bool spacing_and_cover_reqs_okay = false;

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
            steel_yield_str = steel_strength;
            concrete_comp_str = concrete_strength;
            lambda = lightweight ? 0.75 : 1.0;


            if (DEBUG_MODE)
            {
                Console.WriteLine("   Analyzing a #" + bar_size.ToString() + " straight bar with fy=" + steel_yield_str.ToString() + " psi amd f'c=" + concrete_comp_str.ToString() + "psi");
                Console.WriteLine("   Epoxy coated: " + epoxy_coated.ToString() + " : Top bars: " + top_bar_position.ToString());
                Console.WriteLine("   MATS: fy      f'c    lambda");
                Console.WriteLine("         " + steel_yield_str.ToString() +  " : " + concrete_comp_str.ToString() + " : " + lambda.ToString());
            }

            psi_e = epoxy_coated ? 1.3 : 1.0;
            psi_s = bar_size <= 7 ? 0.7 : 1.0;
            psi_t = top_bar_position ? 1.2 : 1.0;
            psi_g = (steel_yield_str <= 60000) ? 1.0 : ((steel_yield_str > 80000) ? 1.15 : 1.30);

            // Apply the limit from ACI318-19 Table 25.4.2.5 for product of psi_e and psi_t greater than 1.7
            psi_e_x_psi_t_product = (psi_e * psi_t > 1.7) ? 1.7 : psi_e * psi_t;
           
            if (DEBUG_MODE)
           {
                string str = "   PSI  E     S     T     G";

                if ((psi_e * psi_t > 1.7))
                    str += " -- Table 25.4.2.5 limit of 1.7 triggered";
                Console.WriteLine(str);
                Console.WriteLine("        " + psi_e.ToString() + " : " + psi_s.ToString() + " : " + psi_t.ToString() + " : " + psi_g.ToString());
            }

            // Compute cb from lesser of half of clear cc spacing or side or bottom covers
            double cb = (new double[] { adj_cc_spacing / 2.0, side_cover, top_cover }).Min();

            // Compute the (cb + Ktr) / db factor and apply limit of 2.5 from ACI318-19 25.4.2.4 
            double cb_plus_Ktr_factor = (((cb + Ktr) / bar_dia) > 2.5 ? 2.5 : (cb + Ktr) / bar_dia);

            if (DEBUG_MODE)
            {
                if (Ktr == 0)
                    Console.WriteLine("   Conservative Ktr=0 used.");

                string str = cb_plus_Ktr_factor.ToString();
                if ((cb + Ktr) / bar_dia >= 2.5)
                    str += ((cb + Ktr) / bar_dia).ToString() + " - but LIMIT TRIGGERED -- 2.5 used";

                Console.WriteLine("   Clear spacing= " + adj_cc_spacing.ToString() + " : side_cover= " + side_cover.ToString() + " : top_cover=" + top_cover.ToString());
                Console.WriteLine("   cb=" + cb.ToString() + " :   (Cb + Ktr) / db = " + str);
            }

            // Checks for ACI318-19 Table 25.4.2.3
            spacing_check = adj_cc_spacing > bar_dia ? true : false;
            clear_check = ((side_cover > bar_dia) && (top_cover > bar_dia)) ? true : false;
            trans_stirrups_throughout_check = trans_reinf_provided;

            if (spacing_check && clear_check && trans_stirrups_throughout_check)
                spacing_and_cover_reqs_okay = true;

            // Determine which equation from Table 25.4.2.3 to use for straight development length
            string devLengthEqString = "";
            if (bar_size <= 6)
            {
                devLengthEqString += "   Bar size 6 and smaller";
                if (spacing_and_cover_reqs_okay)
                {
                    devLengthEqString += " and spacing reqs are okay (ACI318-19 Table 24.4.2.3).";
                    devLength = SmallBarDevLength_SpacingReqsOkay(bar_dia, steel_yield_str, concrete_comp_str, psi_e_x_psi_t_product, psi_g, lambda, DEBUG_MODE);
                } else
                {
                    devLengthEqString += " and spacing reqs exceed limits (ACI318-19 Table 24.4.2.3)";
                    devLength = SmallBarDevLength_SpacingReqsInvalid(bar_dia, steel_yield_str, concrete_comp_str, psi_e_x_psi_t_product, psi_g, lambda, DEBUG_MODE);
                }
            }
            else
            {
                devLengthEqString += "   Bar size greater than 6";
                if (spacing_and_cover_reqs_okay)
                {
                    devLengthEqString += " and spacing reqs are okay (ACI318-19 Table 24.4.2.3).";
                    devLength = LargeBarDevLength_SpacingReqsOkay(bar_dia, steel_yield_str, concrete_comp_str, psi_e_x_psi_t_product, psi_g, lambda, DEBUG_MODE);
                }
                else
                {
                    devLengthEqString += " and spacing reqs exceed limits (ACI318-19 Table 24.4.2.3)";
                    devLength = LargeBarDevLength_SpacingReqsInvalid(bar_dia, steel_yield_str, concrete_comp_str, psi_e_x_psi_t_product, psi_g, lambda, DEBUG_MODE);
                }
            }

            if (DEBUG_MODE)
            {
                Console.WriteLine(devLengthEqString);
            }

            // Tension development length per ACI318-19 Eqn. 25.4.2.4a
            double devLength_allfactors = ((3 * steel_yield_str * psi_s * psi_g * psi_e_x_psi_t_product) / (40.0 * lambda * Math.Sqrt(concrete_comp_str) * cb_plus_Ktr_factor)) * bar_dia;

            if (DEBUG_MODE)
            {
                Console.WriteLine("    ld = " + devLength.ToString() + " vs. " + devLength_allfactors.ToString());
            }

            // Round up the result
            return Math.Min(Math.Ceiling(devLength),Math.Ceiling(devLength_allfactors));
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
