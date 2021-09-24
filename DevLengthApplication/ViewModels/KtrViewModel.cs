using DevLengthApplication.Models;

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
                if(value != null)
                    m_KTRModel = value;

                Update();
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

        public string GetKtrString
        {
            get
            {
                string str = "";

                if (Model != null)
                {
                    if (Model.wasComputed)
                    {
                        str += "Computed ";
                    }
                    else
                    {
                        str += "Assumed ";
                   }
                }
                str += "KTR =" + Model.ComputeKtr().ToString();

                return str;
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
            //bool bValidInput = true;
            //int numbars = 0;
            //string parseMessage = "";
            //if (!Int32.TryParse(MainWindow.spKTRInput.tbNumBars.Text, out numbars))
            //{
            //    bValidInput = false;
            //    parseMessage += "Side Cover must be a valid double\n";
            //}


            OnPropertyChanged("GetWasComputedString");
            OnPropertyChanged("GetN");
            OnPropertyChanged("GetS");
            OnPropertyChanged("GetA_TR");
            OnPropertyChanged("GetKtr");
            OnPropertyChanged("GetKtrString");
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
