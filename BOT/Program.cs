using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

using System.Threading;
namespace BOT
{
    class Program
    {
        static string urlShapeshift = "https://shapeshift.io/";

        static void Main(string[] args)
        {
            Timer t = new Timer(Timer_Elapsed, null, 0, 2000);
            Console.Read();
        }

        private static void Timer_Elapsed(object o)
        {
            //legge info da file per creare degli oggetti
            string sWallet = File.ReadAllText("wallet.txt");
            string sLastSoldAt = File.ReadAllText("lastsoldat.txt");
            Wallet wallet = JsonConvert.DeserializeObject<Wallet>(sWallet); //rappresenta il portafoglio con all'interno la quantità delle varie monete posseduto
            LastSoldAt lastSoldAt = JsonConvert.DeserializeObject<LastSoldAt>(sLastSoldAt); //????

            string activeCoin, unactiveCoin;
            List<string> pairs = new List<string>();
            MarketInfo bestRate;
            ExactPriceResponse exactPrice;
            double dBestRate = -1, rateResult=0;
            
            while (true)
            {
                activeCoin = wallet.getActiveCoin();
                unactiveCoin = wallet.getUnactiveCoin();
                pairs = wallet.getUsablePairs();
                bestRate = getBestRate(pairs);
                dBestRate = bestRate.Rate;
                rateResult = (wallet.getCoinValue(activeCoin) * dBestRate) - bestRate.MinerFee;

                if (rateResult > lastSoldAt.Amount)
                {
                    exactPrice = getExactPrice(rateResult, bestRate.Pair);

                    Console.WriteLine("Sending :" + exactPrice.Success.DepositAmount + " " + activeCoin + " for " + rateResult + " " + unactiveCoin);
                    Console.WriteLine("Wallet (BTC): " + wallet.record["btc"] + " , Wallet (ETH): " + wallet.record["eth"]);
                    Console.WriteLine("Last sold: " + lastSoldAt.Coin + " at: " + lastSoldAt.Amount);
                    File.AppendAllText("log.txt", DateTime.Now + " : " + "Wallet (BTC): " + wallet.record["btc"] + " , Wallet (ETH): " + wallet.record["eth"] + Environment.NewLine);
                    File.AppendAllText("log.txt", DateTime.Now + " : " + "Last sold: " + lastSoldAt.Coin + " at: " + lastSoldAt.Amount + Environment.NewLine);
                    File.AppendAllText("log.txt", DateTime.Now + " : " + "Sending :" + exactPrice.Success.DepositAmount + " " + activeCoin + " for " + rateResult + " " + unactiveCoin + Environment.NewLine);

                    lastSoldAt.Coin = wallet.getActiveCoin();
                    lastSoldAt.Amount = exactPrice.Success.DepositAmount;

                    wallet.send(activeCoin, unactiveCoin, exactPrice.Success.DepositAmount, rateResult);

                    string json = JsonConvert.SerializeObject(lastSoldAt);
                    File.WriteAllText("lastsoldat.txt", json);
                }
            }
        }



        public static ExactPriceResponse getExactPrice(double amount, string pair)
        {
            string result, json;
            using (var client = new WebClient())
            {
                ExactAmountRequest exReq = new ExactAmountRequest();
                exReq.amount = amount;
                exReq.pair = pair;
                json = JsonConvert.SerializeObject(exReq);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                result = client.UploadString(urlShapeshift + "sendamount/", "POST", json);

            }
            return JsonConvert.DeserializeObject<ExactPriceResponse>(result);
        }
        public static MarketInfo getMarketInfo(string pair)
        {
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(urlShapeshift + "marketinfo/" + pair);
            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            string str = objReader.ReadToEnd();
            MarketInfo marketInfo = JsonConvert.DeserializeObject<MarketInfo>(str);


            return marketInfo;
        }

        public static MarketInfo getBestRate(List<string> pairs)
        {
            string bestRate = "NO"; 
            foreach(string pair in pairs)
            {
                if(bestRate == "NO" & getMarketInfo(pair).Rate > 1)
                {
                    bestRate = pair;
                }
                else if(getMarketInfo(pair).Rate > getMarketInfo(bestRate).Rate)
                {
                    bestRate = pair;
                }
            }
            return getMarketInfo(bestRate);
        }
        
    }

    
}
