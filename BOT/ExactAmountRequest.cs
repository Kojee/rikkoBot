using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOT
{
    class ExactAmountRequest
    {
        private double mAmount;
        private string mPair;

        

        public double amount
        {
            get
            {
                return mAmount;
            }
            set
            {
                mAmount = value;
            }
        }

        public string pair
        {
            get
            {
                return mPair;
            }
            set
            {
                mPair = value;
            }
        }
    }
}
