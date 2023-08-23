using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RetrieveData.Models
{
	public class MarketCapData
	{
		public long Timestamp { get; set; }
		public double MarketCap { get; set; }

		public MarketCapData(long timestamp, double marketCap)
		{
			Timestamp = timestamp;
			MarketCap = marketCap;
		}
	}
}
