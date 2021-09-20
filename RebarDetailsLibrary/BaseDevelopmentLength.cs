using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarDetailsLibrary
{
    public enum DevelopmentLengthTypes
    {
        DEV_LENGTH_UNDEFINED = -1,
        DEV_LENGTH_STRAIGHT = 0,
        DEV_LENGTH_HOOKED = 1
    }

    public static class RebarSizes
    {
        public static int[] Size = { 3, 4, 5, 6, 7, 8, 9, 10, 11, 14, 18 };
    }

    public class BaseDevelopmentLength
    {
        public const bool REBAR_DEBUG_MODE = true;

        public bool DEBUG_MODE = !REBAR_DEBUG_MODE;

        public RebarDetailsLibrary.DevelopmentLengthTypes DevLengthType = RebarDetailsLibrary.DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED;

        public List<string> StatusMessageList { get; set; } = new List<string>();

        public int BarSize { get; set; } = 3;
        public double BarDiameter { get; set; }
        public double SteelYieldStrength { get; set; } = 60000;
        public double ConcreteCompStrength { get; set; } = 4000;

        public bool LightWeightConcreteStatus { get; set; } = false;

        public bool EpoxyBarStatus { get; set; } = true;

        public double SideCover { get; set; } = 3;

        public double BottomCover { get; set; } = 3;

        public double CC_Spacing { get; set; } = 3;

        public double Lambda { get; set; }

        public int bar_size;
        public double bar_dia;
        public double steel_yield_str;
        public double concrete_comp_str;
        public double lambda;

        public BaseDevelopmentLength()
        {
            DevLengthType = DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED;
        }

        public BaseDevelopmentLength(
            DevelopmentLengthTypes devType,
            int sizeBar, 
            double fy, 
            double fc, 
            bool mode,
            double cc_spacing ,
            double side,
            double bottom,
            bool lw,
            bool epoxy
            )
        {
            DevLengthType = devType;
            bool DEBUG_MODE = mode;
            BarSize = sizeBar;
            BarDiameter = sizeBar / 8.0;

            // Look for valid rebar size
            bool sizeValid = false;
            foreach(var item in RebarSizes.Size)
            {
                if(BarSize == item)
                {
                    sizeValid = true;
                    break;
                }
            }
            if(!sizeValid)
                throw new InvalidOperationException("ERROR: Invalid bar size - barSize #" + BarSize.ToString());

            if (fy <= 0 || fc <= 0)
                throw new InvalidOperationException("ERROR: Material Strength is invalid - fy=" + steel_yield_str.ToString() + " f'c=" + concrete_comp_str);


            if (cc_spacing < 0 || side < 0 || bottom< 0)
            {
                throw new InvalidOperationException("ERROR: Cover and spacing reqs must be positive = CC_spacing=" + cc_spacing.ToString() + " : side_cover=" + side.ToString() + " : top/bottom cover=" + bottom.ToString());
            }

            StatusMessageList.Add("Bar Size = " + BarSize + " -- dia. = " + BarDiameter);

            SteelYieldStrength = fy;
            ConcreteCompStrength = fc;
            StatusMessageList.Add("fy = " + steel_yield_str + "      f'c = " + concrete_comp_str);

            CC_Spacing = cc_spacing;
            SideCover = side;
            BottomCover = bottom;
            LightWeightConcreteStatus = lw;
            EpoxyBarStatus = epoxy;

            Lambda = LightWeightConcreteStatus ? 0.75 : 1.0;
            StatusMessageList.Add("lambda = " + Lambda.ToString() + " -- " + (LightWeightConcreteStatus ? "" : " NOT ") + "lightweight concrete");
        }

        public virtual BaseDevelopmentLength Compute() { return null; }


        public virtual double DevLength() { return -1; }

        public virtual string DisplayFactors() { return "in base class"; }

    }
}
