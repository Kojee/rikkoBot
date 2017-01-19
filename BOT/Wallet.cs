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

        
        /// <summary>
        /// Ritorna il nome della moneta posseduta in maggior quantità.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Ritorna il nome della moneta posseduta in minor quantità.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Ritorna la lista di tutte le possibili conversioni possibili in base ai tipi di moneta definiti nel formato "from-to".
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Ritorna la lista delle conversioni possibili dalla moneta presente in maggior quantità verso le altre disponibili nel formato "from-to".
        /// </summary>
        /// <returns></returns>
        public List<string> getUsablePairs()
        {
            List<string> pairs = new List<string>();
            string activeCoin = getActiveCoin();
                foreach (KeyValuePair<string, double> to in record)
                    if (activeCoin != to.Key)
                        pairs.Add(activeCoin + "_" + to.Key);
            return pairs;
        }

        /// <summary>
        /// Ritorna la quantità della moneta selezionata presente nel portafoglio.
        /// </summary>
        /// <param name="coin">Nome moneta</param>
        /// <returns></returns>
        public double getCoinValue(string coin)
        {
            return record[coin];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <param name="rateresult"></param>
        public void send(string from, string to, double amount, double rateResult)
        {
            record[from] -= amount;
            record[to] += rateResult;   //non dovrebbe essere rateResult*amount
            
            //scrive nel file wallet.txt che rappresenta quest'oggetto le nuove quantità delle monete presenti dopo lo scambio.
            string upWallet = JsonConvert.SerializeObject(this);
            File.WriteAllText("wallet.txt", upWallet);
            
        }
    }
}
