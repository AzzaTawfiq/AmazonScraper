using AmazonScraper.Core.Interfaces;
using AmazonScraper.Shared.Exceptions;
using System.Net;

namespace AmazonScraper.Infrastructure.Handlers
{
    public class ClientHandler: IClientHandler
    {
        private static readonly List<string> UserAgents = new()
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.2.1 Safari/605.1.15",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/121.0"
        };

        private readonly HttpClient _httpClient;
        public ClientHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetHTMLData(string url)
        {
            //_httpClientHandler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;

            using (_httpClient)
            {
                // 1. Setup Request headers to bypass fundamental bot detection
                var random = new Random();
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgents[random.Next(UserAgents.Count)]);
                _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\", \"Google Chrome\";v=\"120\"");

                var response = await _httpClient.GetAsync(url);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        // Proxy was flagged or Amazon served a CAPTCHA challenge
                        throw new AmazonCaptchaException("Access denied by Amazon. Bot protection page served instead of product markup.");
                    }

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new ProductNotFoundException("");
                    }
                }
                return await response.Content.ReadAsStringAsync();
            }
        }

    }
}
