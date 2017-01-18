using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOT
{
    class LastSoldAt
    {
        private string mCoin;
        private double mAmount;

        public double Amount
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

        public string Coin
        {
            get
            {
                return mCoin;
            }
            set
            {
                mCoin = value;
            }
        }
    }
}
