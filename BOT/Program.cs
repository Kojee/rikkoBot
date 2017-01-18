using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
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

            /*
            while (true)
            {
                Console.Write("> ");
                string requestType = Console.ReadLine();
                switch (requestType)
                {
                    case "WALL":
                        Console.Write(">> ");
                        apiCall = Console.ReadLine();
                        WALLcommands wallCommand = (WALLcommands)Enum.Parse(typeof(WALLcommands), apiCall);
                        switch (wallCommand)
                        {
                            case WALLcommands.check:
                                Console.Write(">>> ");
                                apiCall = apiCall + "/" + Console.ReadLine();
                                break;
                            default:
                                break;
                        }
                        break;
                    case "GET":
                        Console.Write(">> ");
                        apiCall = Console.ReadLine();
                        GETcommands getCommand = (GETcommands)Enum.Parse(typeof(GETcommands), apiCall);
                        switch (getCommand)
                        {
                            case GETcommands.rate:
                                Console.Write(">>> ");
                                apiCall = apiCall + "/" + Console.ReadLine();
                                result = GenericGET(apiCall);
                                break;
                            case GETcommands.limit:
                                Console.Write(">>> ");
                                apiCall = apiCall + "/" + Console.ReadLine();
                                result = GenericGET(apiCall);
                                break;
                            case GETcommands.marketinfo:
                                Console.Write(">>> ");
                                apiCall = apiCall + "/" + Console.ReadLine();
                                result = GenericGET(apiCall);
                                break;
                            case GETcommands.recenttx:
                                Console.Write(">>> ");
                                apiCall = apiCall + "/" + Console.ReadLine();
                                break;
                            case GETcommands.txStat:
                                Console.Write(">>> ");
                                apiCall = apiCall + "/" + Console.ReadLine();
                                break;
                            case GETcommands.timeremaining:
                                Console.Write(">>> ");
                                apiCall = apiCall + "/" + Console.ReadLine();
                                break;
                            case GETcommands.getcoins:
                                break;
                            case GETcommands.validateAddress:
                                Console.Write(">>> ");
                                apiCall = apiCall + "/" + Console.ReadLine();
                                Console.Write(">>> ");
                                apiCall = apiCall + "/" + Console.ReadLine();
                                break;
                            default:
                                break;
                        }
                        
                        break;
                    case "POST":
                        Console.Write(">> ");
                        apiCall = Console.ReadLine();
                        GenericPOST(apiCall);
                        break;
                    case default(string):
                        Console.WriteLine("Error");
                        break;
                }
                
            }*/
        }

        private static void Timer_Elapsed(object o)
        {
            
            string sWallet = File.ReadAllText("wallet.txt");
            string sLastSoldAt = File.ReadAllText("lastsoldat.txt");
            Wallet wallet = JsonConvert.DeserializeObject<Wallet>(sWallet);
            string upWallet = JsonConvert.SerializeObject(wallet);
            LastSoldAt lastSoldAt = JsonConvert.DeserializeObject<LastSoldAt>(sLastSoldAt);
            string apiCall, result;
            string activeCoin, unactiveCoin;
            List<string> pairs = new List<string>();
            MarketInfo bestRate;
            ExactPriceResponse exactPrice;
            double dBestRate = -1, inverseRate, rateResult;
            
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
                    
                    wallet.send(activeCoin, unactiveCoin, exactPrice.Success.DepositAmount, rateResult);

                    lastSoldAt.Coin = wallet.getActiveCoin();
                    lastSoldAt.Amount = exactPrice.Success.DepositAmount;
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
        public static void GenericWALL(string command)
        {

        }

        public static string GenericGET(string command)
        {
            
            
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(urlShapeshift + command);
            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";
            int i = 0;

            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    Console.WriteLine("{0}:{1}", i, sLine);
            }

            return objReader.ReadToEnd();
        }

        public static void GenericPOST(string command)
        {

        }

        public static string getActiveCoin(Wallet wallet)
        {
            return "a";
        }
    }

    enum GETcommands
    {
        rate,
        limit,
        marketinfo,
        recenttx,
        txStat,
        timeremaining,
        getcoins,
        validateAddress
    }

    enum POSTcommands
    {
        shift,
        mail,
        sendamount,
        cancelpending
    }

    enum WALLcommands
    {
        check
    }
}
