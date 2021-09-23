using DevLengthApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevLengthApplication.ViewModels
{
    public class KtrViewModel : BaseViewModel
    {
        private KtrModel m_KTRModel = null;
        // stores the model associated with this object
        public KtrModel Model
        {
            get => m_KTRModel;
            set
            {
                m_KTRModel = value;

                OnPropertyChanged("GetA_TR");
                OnPropertyChanged("GetN");
                OnPropertyChanged("GetS");
                OnPropertyChanged("GetKtr");
            }
        }

        public double GetA_TR
        {
            get
            {
                if (Model != null)
                    return Model.A_TR;
                return -2;
            }
        }

        public int GetN
        {
            get
            {
                if (Model != null)
                    return Model.N;
                return -2;
            }
        }

        public double GetS
        {
            get
            {
                if (Model != null)
                    return Model.S;
                return -2;
            }
        }

        public double GetKtr
        {
            get
            {
                if(Model != null)
                    return Model.ComputeKtr();

                return -2;
            }
        }

        public string GetWasComputedString
        {
            get
            {
                if (Model != null)
                {
                    if (Model.wasComputed)
                        return "Computed";
                    else
                        return "Not computed";
                }
                return "NoModel";
            }
        }

        internal void Update()
        {
            OnPropertyChanged("GetWasComputedString");
            OnPropertyChanged("GetN");
            OnPropertyChanged("GetS");
            OnPropertyChanged("GetA_TR");
            OnPropertyChanged("GetKtr");
        }

        public KtrViewModel()
        {
            Model = new KtrModel();
        }

        public KtrViewModel(int n, double atr, double spa, bool iscalculated)
        {
            Model = new KtrModel(n, atr, spa, iscalculated);
        }
    }
}
