# C# Binance API
This project is an implementation of the Binance API in C# - it is intended to encourage people to start developing projects utilizing this code.

#### Getting Started

The API key and secret can be set in the BinanceClient/Binance.cs file OR be passed into the client

Setting in BinanceClient/Binance.cs
```c#
public class BinanceClient : IBinanceClient
    {
        private readonly HttpClient _httpClient;
        private string url = "https://www.binance.com/api/";
        private string key = "key";
        private string secret = "secret";
```
Initializing the Client/Service
```c#
using BinanceAPI;
using static BinanceAPI.BinanceClient;

var binanceClient = new BinanceAPI.BinanceClient();
var binanceService = new BinanceService(binanceClient);
```
Passing thru to BinanceClient
```c#
using BinanceAPI;
using static BinanceAPI.BinanceClient;

var binanceClient = new BinanceAPI.BinanceClient("key", "secret");
var binanceService = new BinanceService(binanceClient);
```

BinanceExecute/Examples.cs includes the following line in every example:
```c#
Task.WaitAll(getDepth);
```
This is done because the examples are written in a standard console application.


#### Get ticker depth
Signature
```c#
public async Task<dynamic> GetDepthAsync(string symbol)
```
Example
```c#
var getDepth = binanceService.GetDepthAsync("BNBBTC"); 
dynamic depth = getDepth.Result;
Console.WriteLine(depth);
```
#### Get account information
Signature
```c#
public async Task<dynamic> GetAccountAsync()
```
Example
```c#
var getAccount = binanceService.GetAccountAsync();
dynamic account = getAccount.Result;
Console.WriteLine(account);
```
#### Get orders for a symbol
Signature
```c#
public async Task<dynamic> GetOrdersAsync(string symbol, int limit = 500)
```
Example
```c#
var getOrders = binanceService.GetOrdersAsync("BNBBTC");
dynamic orders = getOrders.Result;
Console.WriteLine(orders);
```
#### Get positions (trades) for your account
Signature
```c#
public async Task<dynamic> GetTradesAsync(string symbol)
```
Example
```c#
var getMyTrades = binanceService.GetTradesAsync("WTCBTC");
dynamic trades = getMyTrades.Result;
Console.WriteLine(trades);
```
#### Get all the latest prices for all symbols
Signature
```c#
public async Task<dynamic> GetAllPricesAsync()
```
Example
```c#
var getAllPrices = binanceService.GetAllPricesAsync();           
dynamic prices = binanceService.ListPrices(getAllPrices.Result);
Console.WriteLine(prices);
```
#### Get the price of a symbol
Signature
```c#
public async Task<dynamic> GetOrdersAsync(string symbol, int limit = 500)
```
Example
```c#
var getSymbolPrice = binanceService.GetAllPricesAsync();
dynamic priceList = binanceService.ListPrices(getAllPrices.Result);
double priceOfSymbol = binanceService.GetPriceOfSymbol("BNBBTC", priceList);
Console.WriteLine("Price of BNB: " + priceOfSymbol);
```
#### Place a BUY order
Signature
```c#
public async Task<dynamic> PlaceBuyOrderAsync(string symbol, double quantity, double price, string type = "LIMIT")
```
Example (LIMIT)
```c#
var placeBuyOrder = binanceService.PlaceBuyOrderAsync("NEOBTC", 1.00, 00.008851);
dynamic buyOrderResult = placeBuyOrder.Result;
Console.WriteLine(buyOrderResult);
```
Example (MARKET)
```c#
var placeBuyOrder = binanceService.PlaceBuyOrderAsync("NEOBTC", 1.00, 0, "MARKET");
dynamic buyOrderResult = placeBuyOrder.Result;
Console.WriteLine(buyOrderResult);
```
#### Place a SELL order
Signature
```c#
public async Task<dynamic> PlaceSellOrderAsync(string symbol, double quantity, double price, string type = "LIMIT")
```
Example (LIMIT)
```c#
var placeSellOrder = binanceService.PlaceSellOrderAsync("NEOBTC", 1.00, 00.008851);
dynamic sellOrderResult = placeSellOrder.Result;
Console.WriteLine(sellOrderResult);
```
Example (MARKET)
```c#
var placeSellOrder = binanceService.PlaceSellOrderAsync("NEOBTC", 1.00, 0, "MARKET");
dynamic sellOrderResult = placeSellOrder.Result;
Console.WriteLine(sellOrderResult);
```
#### Check an order's status
Signature
```c#
public async Task<dynamic> CheckOrderStatusAsync(string symbol, int orderId)
```
Example
```c#
var checkOrder = binanceService.CheckOrderStatusAsync("NEOBTC", 5436663);
dynamic checkOrderResult = checkOrder.Result;
Console.WriteLine(checkOrderResult);
```
#### Delete an order
Signature
```c#
public async Task<dynamic> CancelOrderAsync(string symbol, int orderId)
```
Example
```c#
var deleteOrder = binanceService.CancelOrderAsync("NEOBTC", 5436663);
dynamic deleteOrderResult = deleteOrder.Result;
Console.WriteLine(deleteOrderResult);
```
