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

            // TODO: Error check these values.
            SideCover = sidecover;
            BottomCover = topbotcover;
            CC_Spacing = cc_spacing;
            HasMinTransverseReinf = has_mintransversereinf;
            K_TR = ktr;

            PSI_E = RebarDetailsLibrary.DevelopmentLength.ComputePSI_E(EpoxyBarStatus, out status_msg);
            PSI_T = RebarDetailsLibrary.DevelopmentLength.ComputePSI_T(TopBarStatus, out status_msg);
            PSI_S = RebarDetailsLibrary.DevelopmentLength.ComputePSI_S(BarSize, out status_msg);
            PSI_G = RebarDetailsLibrary.DevelopmentLength.ComputePSI_G(SteelYieldStrength, out status_msg);

            DevelopmentLengthStraight = ComputeStraightLength();
        }

        private double ComputeStraightLength()
        {
            List<string> msgList = new List<string>();
            m_developmentLength = RebarDetailsLibrary.DevelopmentLength.Straight(out msgList, BarSize, SteelYieldStrength, ConcreteCompStrength, false, 0, HasMinTransverseReinf, CC_Spacing, SideCover, BottomCover, LightWeightConcreteStatus, EpoxyBarStatus, TopBarStatus);
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

            

            //str += ("Analyzing a #" + BarSize.ToString() + " straight bar\n");
            //str += ("fy=" + SteelYieldStrength.ToString() + " psi\n");
            //str += ("f'c=" + ConcreteCompStrength.ToString() + "psi\n");
            //str += ("Epoxy coated? " + EpoxyBarStatus.ToString() + "\n");
            //str += ("Top bars? " + TopBarStatus.ToString() + "\n");
            //str += ("Lightweight Concrete? " + LightWeightConcreteStatus.ToString() + "\n");
            //str += ("PSI_E: " + PSI_E + "\n");
            //str += ("PSI_S: " + PSI_S + "\n");
            //str += ("PSI_G: " + PSI_G + "\n");
            //str += ("PSI_T: " + PSI_T + "\n");
            //str += ("Side Cover: " + SideCover + "\n");
            //str += ("Top/Bot Cover: " + BottomCover + "\n");
            //str += ("Adj Center Spacing: " + CC_Spacing + "\n");
            //str += ("HasMinTransverse: " + HasMinTransverseReinf + "\n");


            return str;
        }
    }
}
