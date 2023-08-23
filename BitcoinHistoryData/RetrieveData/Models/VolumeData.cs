using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RetrieveData.Models
{
	public class VolumeData
	{
		public long Timestamp { get; set; }
		public double Volume { get; set; }

		public VolumeData(long timestamp, double volume)
		{
			Timestamp = timestamp;
			Volume = volume;
		}
	}
}
