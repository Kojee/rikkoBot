using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOT
{
    class ExactPrice
    {
        private string mPair, mExpiration, mOrderId;
        private double mWithdrawalAmount, mDepositAmount, mQuotedRate, mMinerFee, mLimit;

        public string OrderID
        {
            get
            {
                return mOrderId;
            }
            set
            {
                mOrderId = value;
            }
        }

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
        public double WithdrawalAmount
        {
            get
            {
                return mWithdrawalAmount;
            }
            set
            {
                mWithdrawalAmount = value;
            }
        }

        public double DepositAmount
        {
            get
            {
                return mDepositAmount;
            }
            set
            {
                mDepositAmount = value;
            }
        }
        public string Expiration
        {
            get
            {
                return mExpiration;
            }
            set
            {
                mExpiration = value;
            }
        }

        
        public double QuotedRate
        {
            get
            {
                return mQuotedRate;
            }
            set
            {
                mQuotedRate = value;
            }
        }

        public double MaxLimit
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
