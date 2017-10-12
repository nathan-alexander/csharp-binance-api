using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace BinanceAPI
{
    #region Client
    public interface IBinanceClient
    {
        Task<T> GetAsync<T>(string endpoint, string args = null);
        Task<T> GetSignedAsync<T>(string endpoint, string args = null);
        Task<T> PostSignedAsync<T>(string endpoint, string args = null);
        Task<T> DeleteSignedAsync<T>(string endpoint, string args = null);

    }

    public class BinanceClient : IBinanceClient
    {
        private readonly HttpClient _httpClient;
        private string url = "https://www.binance.com/api/";
        private string key = "key";
        private string secret = "secret";


        public BinanceClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)

            };
            _httpClient.DefaultRequestHeaders
                 .Add("X-MBX-APIKEY", key);

            _httpClient.DefaultRequestHeaders
                    .Accept
                    .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
          
        }

        public BinanceClient(string apiKey, string apiSecret)
        {
            key = apiKey;
            secret = apiSecret;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)

            };
            _httpClient.DefaultRequestHeaders
                 .Add("X-MBX-APIKEY", key);

            _httpClient.DefaultRequestHeaders
                    .Accept
                    .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        // GET
        public async Task<T> GetAsync<T>(string endpoint, string args = null)
        {
            var response = await _httpClient.GetAsync($"{endpoint}?{args}");
            if (!response.IsSuccessStatusCode)
                return default(T);

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);
        }
        //SIGNED GET
        public async Task<T> GetSignedAsync<T>(string endpoint, string args = null)
        {
            string headers = _httpClient.DefaultRequestHeaders.ToString();
            string timestamp = GetTimestamp();
            args += "&timestamp=" + timestamp;
            
            var signature = args.CreateSignature(secret);
            var response = await _httpClient.GetAsync($"{endpoint}?{args}&signature={signature}");
            
            if (!response.IsSuccessStatusCode)
                return default(T);

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);
        }
        //SIGNED POST
        public async Task<T> PostSignedAsync<T>(string endpoint, string args = null)
        {
            string headers = _httpClient.DefaultRequestHeaders.ToString();
            string timestamp = GetTimestamp();
            args += "&timestamp=" + timestamp;

            
            var signature = args.CreateSignature(secret);
            var response = await _httpClient.PostAsync($"{endpoint}?{args}&signature={signature}", null);

            if (!response.IsSuccessStatusCode)
                return default(T);

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);
        }
        //SIGNED DELETE
        public async Task<T> DeleteSignedAsync<T>(string endpoint, string args = null)
        {
            string headers = _httpClient.DefaultRequestHeaders.ToString();
            string timestamp = GetTimestamp();
            args += "&timestamp=" + timestamp;
           
            var signature = args.CreateSignature(secret);
            var response = await _httpClient.DeleteAsync($"{endpoint}?{args}&signature={signature}");

            if (!response.IsSuccessStatusCode)
                return default(T);

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);
        }

        //Timestamp for signature
        private static string GetTimestamp()
        {
            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return milliseconds.ToString();
        }
        //Not in use
        public static string QueryString(IDictionary<string, object> dict)
        {
            var list = new List<string>();
            foreach(var item in dict)
            {
                list.Add(item.Key + "=" + item.Value);
            }
            return string.Join("&", list);
        }
#endregion
    #region Service
        public interface IBinanceService
        {
            //ACCOUNT - GET
            Task<dynamic> GetAccountAsync();
            //MARKET - GET
            Task<dynamic> GetAllPricesAsync();
            Task<dynamic> GetDepthAsync(string symbol);                 
            Task<dynamic> GetTradesAsync(string symbol);
            //ORDERS - GET
            Task<dynamic> GetOrdersAsync(string symbol, int limit);
            Task<dynamic> CheckOrderStatusAsync(string symbol, int orderId);
            //ORDERS - POST
            Task<dynamic> PlaceTestOrderAsync(string symbol, string side, double quantity, double price);
            Task<dynamic> PlaceBuyOrderAsync(string symbol, double quantity, double price, string type);
            Task<dynamic> PlaceSellOrderAsync(string symbol, double quantity, double price, string type);
            //ORDERS - DELETE
            Task<dynamic> CancelOrderAsync(string symbol, int orderId);

            //Task<dynamic> GetCurrentPosition();
            List<Prices> ListPrices(dynamic response);
            double GetPriceOfSymbol(string symbol);
        }

        public class BinanceService : IBinanceService
        {
            private readonly IBinanceClient _binanceClient;

            public BinanceService(IBinanceClient binanceClient)
            {
                _binanceClient = binanceClient;
            }

            //Get depth of a symbol
            public async Task<dynamic> GetDepthAsync(string symbol)
            {
                var result = await _binanceClient.GetAsync<dynamic>("v1/depth", "symbol=" + symbol);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //Get latest price of all symbols
            public async Task<dynamic> GetAllPricesAsync()
            {
                
                var result = await _binanceClient.GetAsync<dynamic>("v1/ticker/allPrices");
                if (result == null)
                {
                    throw new Exception();
                }

                return result;

            }

            //Get account information
            public async Task<dynamic> GetAccountAsync()
            {
                var result = await _binanceClient.GetSignedAsync<dynamic>("v3/account");
                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }

            //Get current positions
            public async Task<dynamic> GetAccountPositionsAsync()
            {
                var result = await _binanceClient.GetSignedAsync<dynamic>("v3/account");
                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //Get list of open orders
            public async Task<dynamic> GetOrdersAsync(string symbol, int limit = 500)
            {            
                var result = await _binanceClient.GetSignedAsync<dynamic>("v3/allOrders", "symbol=" + symbol + "&" + "limit=" + limit);
                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }

            //Get list of trades for account
            public async Task<dynamic> GetTradesAsync(string symbol)
            {
                var result = await _binanceClient.GetSignedAsync<dynamic>("v3/myTrades", "symbol=" + symbol);
                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //Test LIMIT order
            public async Task<dynamic> PlaceTestOrderAsync(string symbol, string side, double quantity, double price)
            {
                var result = await _binanceClient.PostSignedAsync<dynamic>("v3/order/test", "symbol=" + symbol + "&" + "side=" + side + "&" + "type=LIMIT" + "&" + "quantity=" + quantity.ToString() + "&" + "price=" + price.ToString() + "&" + "timeInForce=GTC" + "&" + "recvWindow=6000");
                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }

            //Place a BUY order, defaults to LIMIT if type is not specified
            public async Task<dynamic> PlaceBuyOrderAsync(string symbol, double quantity, double price, string type = "LIMIT")
            {
                var result = await _binanceClient.PostSignedAsync<dynamic>("v3/order", "symbol=" + symbol + "&" + "side=BUY" + "&" + "type=" + type + "&" + "quantity=" + quantity.ToString() + "&" + "price=" + price.ToString() + "&" + "timeInForce=GTC" + "&" + "recvWindow=6000");
                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //Place a SELL order, defaults to LIMIT if type is not specified
            public async Task<dynamic> PlaceSellOrderAsync(string symbol, double quantity, double price, string type = "LIMIT")
            {
                var result = await _binanceClient.PostSignedAsync<dynamic>("v3/order", "symbol=" + symbol + "&" + "side=SELL" + "&" + "type=" + type + "&" + "quantity=" + quantity.ToString() + "&" + "price=" + price.ToString() + "&" + "timeInForce=GTC" + "&" + "recvWindow=6000");
                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }

            //Check an order's status
            public async Task<dynamic> CheckOrderStatusAsync(string symbol, int orderId)
            {
                var result = await _binanceClient.GetSignedAsync<dynamic>("v3/order", "symbol=" + symbol + "&" + "orderId=" + orderId);
                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }


            //Cancel an order
            public async Task<dynamic> CancelOrderAsync(string symbol, int orderId)
            {
                var result = await _binanceClient.DeleteSignedAsync<dynamic>("v3/order", "symbol=" + symbol + "&" + "orderId=" + orderId);
                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }

            //return a List of Price information for consumption
            public List<Prices> ListPrices(dynamic response)
            {
                List<Prices> prices = new List<Prices>();
                prices = JsonConvert.DeserializeObject<List<Prices>>(response.ToString());
                return prices;

            }
            public double GetPriceOfSymbol(string symbol)
            {
                List<Prices> prices = new List<Prices>();
                var task = Task.Run(async () => await _binanceClient.GetAsync<dynamic>("v1/ticker/allPrices"));
                dynamic result = task.Result;
                
                prices = ListPrices(result);

                double priceOfSymbol = (from p in prices
                                     where p.Symbol == symbol
                                     select p.Price).First();

                return priceOfSymbol;
            }

        }


    }
    #endregion
    #region Helpers
    public static class BinanceHelpers
    {
        private static readonly Encoding SignatureEncoding = Encoding.UTF8;

        public static string CreateSignature(this string message, string secret)
        {

            byte[] keyBytes = SignatureEncoding.GetBytes(secret);
            byte[] messageBytes = SignatureEncoding.GetBytes(message);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);

            byte[] bytes = hmacsha256.ComputeHash(messageBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
     
    }

    public class Prices
    {
        public string Symbol { get; set; }
        public double Price { get; set; }
    }
#endregion


}
