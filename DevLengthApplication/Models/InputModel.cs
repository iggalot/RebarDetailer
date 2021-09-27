﻿using ACI318_19Library.DevelopmentLength;

namespace DevLengthApplication.Models
{
    public class InputModel
    {
        public BaseDevelopmentLength DevelopmentLengthObject { get; set; }
        public KtrModel Ktr_Model {get; set;} = null;

        public double LD {get; set;}

        public InputModel() : this(DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED, null)
        {
            Ktr_Model = null;
        }

        public InputModel(ACI318_19Library.DevelopmentLength.DevelopmentLengthTypes type, KtrModel ktrmodel)
        {
            Ktr_Model = (ktrmodel == null) ? new KtrModel() : ktrmodel;

            switch (type)
            {
                case DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED:
                    DevelopmentLengthObject = new BaseDevelopmentLength();
                    break;
                case DevelopmentLengthTypes.DEV_LENGTH_STRAIGHT:
                    DevelopmentLengthObject = new StraightDevelopmentLength(4, 60000, 3000, false, ktrmodel.ComputeKtr());
                    break;
                case DevelopmentLengthTypes.DEV_LENGTH_STANDARD_HOOK:
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
        public InputModel(DevelopmentLengthTypes type, int size, double yield, double comp, bool epoxy, bool topbar, bool lightweight, double sidecover, double topbotcover, double cc_spacing, bool has_mintransversereinf, KtrModel ktr_model, bool terminate_in_column)
        {
            Ktr_Model = ktr_model == null ? new KtrModel() : ktr_model;

            switch (type)
            {
                case DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED:
                    break;
                case DevelopmentLengthTypes.DEV_LENGTH_STRAIGHT:
                    DevelopmentLengthObject = new StraightDevelopmentLength(size, yield, comp, false, Ktr_Model.ComputeKtr(), Ktr_Model.wasComputed, has_mintransversereinf, cc_spacing, sidecover, topbotcover, lightweight, epoxy, topbar);

                    //this.ComputeDevelopmentLength();
                    break;
                case DevelopmentLengthTypes.DEV_LENGTH_STANDARD_HOOK:
                    DevelopmentLengthObject = new HookDevelopmentLength(size, yield, comp, false, HookTypes.HOOK_STANDARD, 90, cc_spacing, sidecover, topbotcover, lightweight, epoxy, terminate_in_column);

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

        public string DisplayDevelopmentFactors()
        {
            string str = "";
            if (DevelopmentLengthObject != null)
            {
                if (DevelopmentLengthObject.StatusMessageList == null)
                    str += "No status messages";
                else
                    str += DevelopmentLengthObject.StatusMessagesDisplayString();
            }
            return str;
        }
    }
}
