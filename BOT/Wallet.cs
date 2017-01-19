using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace BOT
{
    class Wallet
    {

        public IDictionary<string, double> record = new Dictionary<string, double>();

        

        public string getActiveCoin()
        {
            if(record["btc"] > record["eth"])
            {
                return "btc";

            }
            else if(record["btc"] < record["eth"])
            {
                return "eth";
            }
            return "Active coin not found";
        }

        public string getUnactiveCoin()
        {
            if (record["btc"] < record["eth"])
            {
                return "btc";

            }
            else if(record["btc"] > record["eth"])
            {
                return "eth";
            }
            return "Unactive coin not found";
        }

        public List<string> getPairs()
        {
            List<string> pairs = new List<string>();
            foreach(KeyValuePair<string, double> from in record)
            {
                foreach (KeyValuePair<string, double> to in record)
                {
                    if(from.Key != to.Key)
                    {
                        pairs.Add(from.Key + "_" + to.Key);
                    }
                }
            }
            return pairs;
        }

        public List<string> getUsablePairs()
        {
            List<string> pairs = new List<string>();
            string activeCoin = getActiveCoin();
            foreach (KeyValuePair<string, double> from in record)
            {
                foreach (KeyValuePair<string, double> to in record)
                {
                    if (from.Key == activeCoin & from.Key != to.Key)
                    {
                        pairs.Add(from.Key + "_" + to.Key);
                    }
                }
            }
            return pairs;
        }

        public double getCoinValue(string coin)
        {
            return record[coin];
        }

        public void send(string from, string to, double amount, double rateresult)
        {
            record[from] -= amount;
            record[to] += rateresult;
            
            string upWallet = JsonConvert.SerializeObject(this);
            File.WriteAllText("wallet.txt", upWallet);
            
        }
    }
}
