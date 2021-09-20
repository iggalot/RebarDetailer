using RebarDetailsLibrary;
using System.Collections.Generic;

namespace DevLengthApplication.Models
{
    public class InputModel
    {

        public BaseDevelopmentLength DevelopmentLengthObject { get; set; }

        public double LD {get; set;}

        public InputModel() : this(DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED)
        {
        }

        public InputModel(RebarDetailsLibrary.DevelopmentLengthTypes type)
        {
            switch (type)
            {
                case DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED:
                    DevelopmentLengthObject = new BaseDevelopmentLength();
                    break;
                case DevelopmentLengthTypes.DEV_LENGTH_STRAIGHT:
                    DevelopmentLengthObject = new StraightDevelopmentLength(4, 60000, 3000, false);
                    break;
                case DevelopmentLengthTypes.DEV_LENGTH_HOOKED:
                    break;
                default:
                    break;
            }

            LD = DevelopmentLengthObject.DevLength();
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
        public InputModel(DevelopmentLengthTypes type, int size, double yield, double comp, bool epoxy, bool topbar, bool lightweight, double sidecover, double topbotcover, double cc_spacing, bool has_mintransversereinf, double ktr)
        {
            switch (type)
            {
                case DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED:
                    break;
                case DevelopmentLengthTypes.DEV_LENGTH_STRAIGHT:
                    DevelopmentLengthObject = new StraightDevelopmentLength(size, yield, comp, false, ktr, has_mintransversereinf, cc_spacing, sidecover, topbotcover, lightweight, epoxy, topbar);

                    //this.ComputeDevelopmentLength();
                    break;
                case DevelopmentLengthTypes.DEV_LENGTH_HOOKED:
                    break;
                default:
                    {
                        break;
                    }            
            }
            LD = DevelopmentLengthObject.DevLength();
        }

        public InputModel(BaseDevelopmentLength obj)
        {
            DevelopmentLengthObject = obj;
            LD = DevelopmentLengthObject.DevLength();
        }

        public BaseDevelopmentLength ComputeDevelopmentLength()
        {
            //if (DevelopmentLengthObject != null)
            //    return DevelopmentLengthObject.Compute();

            return null;

        }

        public string DisplaytDevelopmentFactors()
        {
            string str = "";
            if (DevelopmentLengthObject != null)
            {
                if (DevelopmentLengthObject.StatusMessageList == null)
                    str += "No status messages";
                else
                    str += DevelopmentLengthObject.DisplayFactors();
            }
            return str;
        }
    }
}
