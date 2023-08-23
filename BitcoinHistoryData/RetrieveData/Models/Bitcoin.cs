using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RetrieveData.Models
{
	public class Bitcoin
    {
		public List<PriceData> PriceHistory { get; set; }

		public List<MarketCapData> MarketCapHistory { get; set; }

		public List<VolumeData> VolumeHistory { get; set; }
	}
}
