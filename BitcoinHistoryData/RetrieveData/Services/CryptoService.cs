using RetrieveData.Models;
using Newtonsoft.Json;

namespace RetrieveData.Services
{
	public class CryptoService : ICryptoService
	{
		public CryptoService() { }

		/// <summary>Fetch data from given API url</summary>
		/// <param name="apiUrl">The API endpoint</param>
		/// <returns>The data from the API endpoint</returns>
		public async Task<Bitcoin> FetchDataAsync(string apiUrl)
		{
			var json = await DownloadWebPageContentAsync(apiUrl);

			var intermediate = JsonConvert.DeserializeObject<IntermediateBitcoin>(json);

			var bitcoin = new Bitcoin
			{
				PriceHistory = new List<PriceData>(),
				MarketCapHistory = new List<MarketCapData>(),
				VolumeHistory = new List<VolumeData>()
			};

			if (intermediate != null)
			{
				for (int i = 0; i < intermediate.Prices.Count; i++)
				{
					bitcoin.PriceHistory.Add(new PriceData((long)intermediate.Prices[i][0], intermediate.Prices[i][1]));
					bitcoin.MarketCapHistory.Add(new MarketCapData((long)intermediate.MarketCaps[i][0], intermediate.MarketCaps[i][1]));
					bitcoin.VolumeHistory.Add(new VolumeData((long)intermediate.TotalVolumes[i][0], intermediate.TotalVolumes[i][1]));
				}
				return bitcoin;
			}

			return bitcoin;
		}

		/// <summary>Download the content of a webpage</summary>
		/// <param name="url">The url of the webpage</param>
		/// <returns>The content of the webpage</returns>
		private async Task<string> DownloadWebPageContentAsync(string url)
		{
			try
			{
				using var httpClient = new HttpClient();
				using var response = await httpClient.GetAsync(url);
				return await response.Content.ReadAsStringAsync();
			}

			catch (Exception e)
			{
				throw new Exception($"Failed to download the content of the webpage: {e.Message}");
			}
		}
	}
}