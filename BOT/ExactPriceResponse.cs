using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOT
{
    class ExactPriceResponse
    {
        private ExactPrice mSuccess;

        public ExactPrice Success
        {
            get
            {
                return mSuccess;
            }
            set
            {
                mSuccess = value;
            }
        }
    }
}
