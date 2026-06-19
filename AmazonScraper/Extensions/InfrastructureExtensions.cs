using AmazonScraper.Core.Interfaces;
using AmazonScraper.Infrastructure.Handlers;
using AmazonScraper.Infrastructure.Scraper;

namespace AmazonScraper.Api.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register your dependencies here
            services.AddScoped<IScraperService, ScraperService>();
            services.AddScoped<IClientHandler, ClientHandler>();
            // Registers both your custom service and its configured HttpClient
            services.AddHttpClient<IClientHandler, ClientHandler>(client =>
            {
                //client.BaseAddress = new Uri("https://www.amazon.com/s?k=all&ref=nb_sb_noss");
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                // Automatically handles GZip and Brotli compression
                AutomaticDecompression = System.Net.DecompressionMethods.All
                 //System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
            });

            // Always return the services collection to allow method chaining
            return services;
        }
    }
}
