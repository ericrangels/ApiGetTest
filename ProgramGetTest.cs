using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Globalization;
using Newtonsoft.Json;

namespace ApiGetTest
{
    public class TransactionResponse
    {
        public string page { get; set; }
        public int per_page { get; set; }
        public int total { get; set; }
        public int total_pages { get; set; }
        public List<Data> data { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public long timestamp { get; set; }
        public string txnType { get; set; }
        public string amount { get; set; }
        public Location location { get; set; }
        public string ip { get; set; }
    }

    public class Location
    {
        public int id { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public long zipCode { get; set; }
    }

    public class DataPart
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string txnType { get; set; }
        public decimal amount { get; set; }
        public int locationId { get; set; }
        public string locationCity { get; set; }
    }


    public class ProgramGetTest
    {

        static void Main(string[] args)
        {

            List<DataPart> dataPart = ProgramGetTest.GetTransactionSummary("debit", "Ilchester");

            foreach(var item in dataPart)
            {
                Console.WriteLine("_____________________________________________");
                Console.WriteLine("userId = " + item.userId.ToString());
                Console.WriteLine("userName = " + item.userName);
                Console.WriteLine("txnType = " + item.txnType);
                Console.WriteLine("amount = " + item.amount.ToString());
                Console.WriteLine("locationId = " + item.locationId.ToString());
                Console.WriteLine("locationCity = " + item.locationCity);
                Console.WriteLine("|");
            }
        }

        static List<DataPart> GetTransactionSummary(string type, string locationCity)
        {
            TransactionResponse deserializedTransaction = new TransactionResponse();

            var transDataPart = new List<DataPart>();

            try
            {
                    string url = "https://jsonmock.hackerrank.com/api/transactions/search?txnType=" + type;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    request.ContentType = "application/json";

                    WebResponse webResponse = request.GetResponse();
                    Stream stream = webResponse.GetResponseStream();
                    StreamReader streamReader = new StreamReader(stream);
                    string response = streamReader.ReadToEnd();

                    deserializedTransaction = JsonConvert.DeserializeObject<TransactionResponse>(response);

                    streamReader.Close();

                    foreach (var item in deserializedTransaction.data.Where(t => t.location.city == locationCity).ToList())
                    {
                        transDataPart.Add(new DataPart { userId = item.userId, userName = item.userName, amount = decimal.Parse(item.amount.Replace("$", ""), NumberStyles.Currency, CultureInfo.InvariantCulture), txnType = item.txnType, locationId = item.location.id, locationCity = item.location.city });
                    }

            }
            catch (Exception e)
            {
                throw new Exception("Error-" + e.Message);
            }


            return transDataPart;
        }
    }
}
