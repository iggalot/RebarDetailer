using System.Collections.Generic;

namespace DevLengthApplication.Models
{
    public class InputModel 
    {
        private double m_developmentLength = 0;
        private double m_sideCover = 3;
        private double m_clearSpacing = 3;
        private double m_bottomCover = 3;

        private List<string> m_StatusMessageList = null;

        public List<string> StatusMessageList
        {
            get => m_StatusMessageList;
            set
            {
                m_StatusMessageList = value;
            }
        }
        public int BarSize { get; set; } = 3;

        public double BarDiameter { get; set; }
        public double SteelYieldStrength { get; set; } = 60000;
        public double ConcreteCompStrength { get; set; } = 4000;

        public bool EpoxyBarStatus { get; set; } = true;
        public bool TopBarStatus { get; set; } = true;
        public bool LightWeightConcreteStatus { get; set; } = false;

        public double PSI_E { get; set; } = 1.0;
        public double PSI_G { get; set; } = 1.0;
        public double PSI_T { get; set; } = 1.0;
        public double PSI_S { get; set; } = 1.0;

        public double SideCover
        {
            get => m_sideCover;
            set
            {
                m_sideCover = value;
            }
        }

        public double BottomCover
        {
            get => m_bottomCover;
            set
            {
                m_bottomCover = value;
            }
        }

        public double CC_Spacing
        {
            get => m_clearSpacing;
            set
            {
                m_clearSpacing = value;
            }
        }
        public bool HasMinTransverseReinf { get; set; } = false;
        public double K_TR { get; set; } = 0;

        public double DevelopmentLengthStraight
        {
            get => m_developmentLength;
            set
            {
                m_developmentLength = value;
            }
        }

        public InputModel()
        {
            // uses the default values
            DevelopmentLengthStraight = ComputeStraightLength();
        }

        /// <summary>
        /// Constructor for the model.  Computes development length for the loaded parameters.
        /// </summary>
        /// <param name="size">rebar size number</param>
        /// <param name="yield">steel yield stress, psi</param>
        /// <param name="comp">concrete compressive stress, psi</param>
        /// <param name="epoxy">is the bar epoxy coated</param>
        /// <param name="topbar">does the bar have more than 12 inches of fresh concrete below it?</param>
        /// <param name="lightweight">is the concrete lightweight?</param>
        /// <param name="sidecover">sidecover distance</param>
        /// <param name="topbotcover">top / bottom cover distance</param>
        /// <param name="cc_spacing">smallest center to center spacing between bars</param>
        /// <param name="has_mintransversereinf">will transverse reinforcement be provided</param>
        /// <param name="ktr">ktr calculation, assumed to be zero</param>
        public InputModel(int size, double yield, double comp, bool epoxy, bool topbar, bool lightweight, double sidecover, double topbotcover, double cc_spacing, bool has_mintransversereinf, double ktr)
        {
            string status_msg;

            BarSize = size;
            BarDiameter = ((double)BarSize) / 8.0;

            SteelYieldStrength = yield;
            ConcreteCompStrength = comp;
            EpoxyBarStatus = epoxy;
            TopBarStatus = topbar;
            LightWeightConcreteStatus = lightweight;

            SideCover = sidecover;
            BottomCover = topbotcover;
            CC_Spacing = cc_spacing;
            HasMinTransverseReinf = has_mintransversereinf;
            K_TR = ktr;

            PSI_E = RebarDetailsLibrary.StraightDevelopmentLength.ComputePSI_E(EpoxyBarStatus, BarDiameter, CC_Spacing, SideCover, BottomCover, out status_msg);
            PSI_T = RebarDetailsLibrary.StraightDevelopmentLength.ComputePSI_T(TopBarStatus, out status_msg);
            PSI_S = RebarDetailsLibrary.StraightDevelopmentLength.ComputePSI_S(BarSize, out status_msg);
            PSI_G = RebarDetailsLibrary.StraightDevelopmentLength.ComputePSI_G(SteelYieldStrength, out status_msg);

            DevelopmentLengthStraight = ComputeStraightLength();
        }

        private double ComputeStraightLength()
        {
            List<string> msgList = new List<string>();
            m_developmentLength = RebarDetailsLibrary.StraightDevelopmentLength.Straight(out msgList, BarSize, SteelYieldStrength, ConcreteCompStrength, false, 0, HasMinTransverseReinf, CC_Spacing, SideCover, BottomCover, LightWeightConcreteStatus, EpoxyBarStatus, TopBarStatus);
            StatusMessageList = msgList;

            return m_developmentLength;
        }

        public string DisplayStraightDevelopmentFactors()
        {
            string str = "";
            for (int i = 0; i < StatusMessageList.Count; i++)
            {
                str += StatusMessageList[i] + "\n";
            }

            return str;
        }
    }
}
