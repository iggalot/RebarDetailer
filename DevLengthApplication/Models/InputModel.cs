using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevLengthApplication.Models
{
    public class InputModel
    {
        private double m_developmentLength = 0;

        public int BarSize { get; set; } = 3;
        public double SteelYieldStrength { get; set; } = 60000;
        public double ConcreteCompStrength { get; set; } = 4000;

        public bool EpoxyBarStatus { get; set; } = true;
        public bool TopBarStatus { get; set; } = true;
        public bool LightWeightConcreteStatus { get; set; } = false;

        public double PSI_E { get; set; } = 1.0;
        public double PSI_G { get; set; } = 1.0;
        public double PSI_T { get; set; } = 1.0;
        public double PSI_S { get; set; } = 1.0;



        public double SideCover { get; set; } = 3.0;
        public double BottomCover { get; set; } = 3.0;
        public double CC_Spacing { get; set; } = 3.0;
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

        public InputModel(int size, int yield, int comp, bool epoxy, bool topbar, bool lightweight, double sidecover, double topbotcover, double cc_spacing, bool has_mintransversereinf, double ktr)
        {
            BarSize = size;
            SteelYieldStrength = yield;
            ConcreteCompStrength = comp;
            EpoxyBarStatus = epoxy;
            TopBarStatus = topbar;
            LightWeightConcreteStatus = lightweight;

            // TODO: Error check these values.
            SideCover = sidecover;
            BottomCover = topbotcover;
            CC_Spacing = cc_spacing;
            HasMinTransverseReinf = has_mintransversereinf;
            K_TR = ktr;

            PSI_E = RebarDetailsLibrary.DevelopmentLength.ComputePSI_E(EpoxyBarStatus);
            PSI_T = RebarDetailsLibrary.DevelopmentLength.ComputePSI_T(TopBarStatus);
            PSI_S = RebarDetailsLibrary.DevelopmentLength.ComputePSI_S(BarSize);
            PSI_G = RebarDetailsLibrary.DevelopmentLength.ComputePSI_G(SteelYieldStrength);



            DevelopmentLengthStraight = ComputeStraightLength();
        }

        private double ComputeStraightLength()
        {
            m_developmentLength = RebarDetailsLibrary.DevelopmentLength.Straight(BarSize, SteelYieldStrength, ConcreteCompStrength, true, 0, false, 3, 3, 3, LightWeightConcreteStatus, EpoxyBarStatus, TopBarStatus);
            return m_developmentLength;
        }

        public string DisplayStraightDevelopmentFactors()
        {
            string str= "";
            str += ("Analyzing a #" + BarSize.ToString() + " straight bar\n");
            str += ("fy=" + SteelYieldStrength.ToString() + " psi\n");
            str += ("f'c=" + ConcreteCompStrength.ToString() + "psi\n");
            str += ("Epoxy coated? " + EpoxyBarStatus.ToString() + "\n");
            str += ("Top bars? " + TopBarStatus.ToString() + "\n");
            str += ("Lightweight Concrete? " + LightWeightConcreteStatus.ToString() + "\n");
            str += ("PSI_E: " + PSI_E + "\n");
            str += ("PSI_S: " + PSI_S + "\n");
            str += ("PSI_G: " + PSI_G + "\n");
            str += ("PSI_T: " + PSI_T + "\n");
            str += ("Side Cover: " + SideCover + "\n");
            str += ("Top/Bot Cover: " + BottomCover + "\n");
            str += ("Adj Center Spacing: " + CC_Spacing + "\n");
            str += ("HasMinTransverse: " + HasMinTransverseReinf + "\n");


            return str;
        }
    }
}
