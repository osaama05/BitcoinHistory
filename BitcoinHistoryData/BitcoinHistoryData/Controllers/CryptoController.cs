using RetrieveData.Services;
using RetrieveData.Models;

namespace BitcoinHistoryData.Controllers
{
	public class CryptoController
	{
		private readonly ICryptoService _service;
		public CryptoController(ICryptoService service) 
		{
			_service = service;
		}

		/// <summary>Convert a DateTime object to unix format</summary>
		/// <param name="dateTime">The time to convert</param>
		/// <returns>The time in unix format</returns>
		public long ToUnixTime(DateTime dateTime)
		{
			DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
			return dto.ToUnixTimeSeconds();
		}

		/// <summary>Convert a unix time to DateTime </summary>
		/// <param name="unixTime">The time in unix format</param>
		/// <returns>The time in DateTime format</returns>
		public DateTime UnixTimeToDateTime(long unixTime)
		{
			DateTimeOffset dto = DateTimeOffset.FromUnixTimeMilliseconds(unixTime);
			return dto.UtcDateTime;
		}

		/// <summary>Get data from given API url</summary>
		/// <param name="apiUrl">The API endpoint</param>
		///<returns>The data from the API endpoint</returns>
		public async Task<Bitcoin> GetDataAsync(DateTime fromDate, DateTime toDate)
		{
            // Take 90 days before fromDate to only get one data point for each day
            var unixTimeFrom = ToUnixTime(fromDate.AddDays(-90));
            // Add one day to get the last given date
            var unixTimeTo = ToUnixTime(toDate.AddDays(1));
            
            string apiUrl = $"https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&from={unixTimeFrom}&to={unixTimeTo}";
            var bitcoinData = await _service.FetchDataAsync(apiUrl);
            
			//Remove first 90 days (only used to get one data point for each day)
			bitcoinData.PriceHistory.RemoveRange(0, 90);
			bitcoinData.VolumeHistory.RemoveRange(0, 90);
			bitcoinData.MarketCapHistory.RemoveRange(0, 90);
            
			return bitcoinData;
		}

		/// <summary>Searches the longest downwards trend in a bitcoin object</summary>
		/// <param name="btc">The Bitcoin object</param>
		///<returns>The length of the bearish</returns>
		public int GetLongestBearish(Bitcoin btc)
		{
			int longestBearish = 0;
			int currentBearish = 0;
			for (int i = 0; i < btc.PriceHistory.Count; i++)
			{
				if (i == 0)
				{
					currentBearish++;
				}

				else if (btc.PriceHistory[i].Price < btc.PriceHistory[i-1].Price || i == 0)
				{
					currentBearish++;
				}

				else
				{
					if (currentBearish > longestBearish)
					{
						longestBearish = currentBearish;
					}
					currentBearish = 0;
				}
			}
			return longestBearish;
		}


		/// <summary>Searches the highest volume in a bitcoin object</summary>
		/// <param name="btc">The Bitcoin object</param>
		/// <returns>A tuple with the highest volume and the date when it was achieved</returns>
		public (DateTime date, double volume) GetHighestVolume(Bitcoin btc)
		{
			DateTime highestVolumeDate = new DateTime();
			double highestVolume = 0;
			for (int i = 0; i < btc.PriceHistory.Count; i++)
			{
				if (btc.VolumeHistory[i].Volume > highestVolume)
				{
					highestVolume = btc.VolumeHistory[i].Volume;
					highestVolumeDate = UnixTimeToDateTime(btc.VolumeHistory[i].Timestamp);
				}
			}
			return (highestVolumeDate, highestVolume);
		}


		/// <summary>Searches the best possible profit of a bitcoin object</summary>
		/// <param name="btc">The Bitcoin object</param>
		///<returns>A tuple with the highest profit possible, as well as the dates the user should buy and sell the coin</returns>
		public (DateTime BuyDate, DateTime SellDate, double Profit) GetBestProfit(Bitcoin btc)
		{
			DateTime bestBuyDate = DateTime.MinValue;
			DateTime bestSellDate = DateTime.MinValue;
			double highestProfit = 0;

			for (int i = 0; i < btc.PriceHistory.Count - 1; i++)
			{
				var currentPrice = btc.PriceHistory[i].Price;

				for (int j = i + 1; j < btc.PriceHistory.Count; j++)
				{
					var sellPrice = btc.PriceHistory[j].Price;
					var profit = sellPrice - currentPrice;

					//If profit is higher than highestProfit, set new highestProfit and buy/sell dates
					if (profit > highestProfit)
					{
						highestProfit = profit;
						bestBuyDate = UnixTimeToDateTime(btc.PriceHistory[i].Timestamp);
						bestSellDate = UnixTimeToDateTime(btc.PriceHistory[j].Timestamp);
					}
				}
			}
			return (bestBuyDate, bestSellDate, highestProfit);
		}
	}
}