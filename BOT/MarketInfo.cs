using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOT
{
    class MarketInfo
    {
        private string mPair;
        private double mRate, mLimit, mMin, mMinerFee;

        public string Pair
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

        public double Rate
        {
            get
            {
                return mRate;
            }
            set
            {
                mRate = value;
            }
        }

        public double Limit
        {
            get
            {
                return mLimit;
            }
            set
            {
                mLimit = value;
            }
        }

        public double Min
        {
            get
            {
                return mMin;
            }
            set
            {
                mMin = value;
            }
        }

        public double MinerFee
        {
            get
            {
                return mMinerFee;
            }
            set
            {
                mMinerFee = value;
            }
        }

    }
}
