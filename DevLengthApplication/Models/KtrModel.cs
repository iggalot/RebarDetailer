using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevLengthApplication.Models
{
    public class KtrModel
    {
        public const double DEFAULT_KTR = 0;
        public const double DEFAULT_ATR = 0;
        public const int DEFAULT_N = -1;
        public const double DEFAULT_S = -1;
        public const bool DEFAULT_WASCOMPUTED = false;

        public bool wasComputed { get; set; } = DEFAULT_WASCOMPUTED;
        public double A_TR { get; set; } = DEFAULT_ATR;
        public int N { get; set; } = DEFAULT_N;
        public double S { get; set; } = DEFAULT_S;

        public double KTR { 
            get; 
            set; 
        } = DEFAULT_KTR;

        public KtrModel(int n, double atr, double s, bool computed_status = false)
        {
            wasComputed = computed_status;
            N = n;
            A_TR = atr;
            S = s;
        }

        public KtrModel()
        {
        }

        /// <summary>
        /// Computes the value of K_TR per 25.4.2.4b
        /// </summary>
        /// <returns></returns>
        public double ComputeKtr()
        {
            if (!IsDefault())
            {
                wasComputed = true;
                return (40 * A_TR / (S * N));
            }
            else
            {
                wasComputed = false;
                // return conservative default
                return 0;
            }
        }

        public bool IsDefault()
        {
            bool result = ((A_TR == DEFAULT_ATR) && (N == DEFAULT_N) && (S == DEFAULT_S));
            return result;
        }
    }
}
