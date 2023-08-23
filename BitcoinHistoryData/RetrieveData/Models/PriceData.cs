namespace RetrieveData.Models
{
	public class PriceData
	{
		public long Timestamp { get; set; }
		public double Price { get; set; }

		public PriceData(long timestamp, double price)
		{
			Timestamp = timestamp;
			Price = price;
		}
	}
}
