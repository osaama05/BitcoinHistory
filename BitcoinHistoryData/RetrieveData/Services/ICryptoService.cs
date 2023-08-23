using RetrieveData.Models;

namespace RetrieveData.Services
{
	public interface ICryptoService
	{
		Task<Bitcoin> FetchDataAsync(string apiUrl);
	}
}