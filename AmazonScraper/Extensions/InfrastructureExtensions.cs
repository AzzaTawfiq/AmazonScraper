using AmazonScraper.Core.Entities;
using AmazonScraper.Core.Interfaces;
using AmazonScraper.Infrastructure.Handlers;
using AmazonScraper.Infrastructure.Scraper;
using static AmazonScraper.Core.Interfaces.IScrapingStrategy;

namespace AmazonScraper.Api.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register your dependencies here
            services.AddTransient<IScraperService<List<Product>>, ScraperService<List<Product>>>();
            services.AddTransient<IScraperService<Product>, ScraperService<Product>>();
            services.AddTransient<IScraperService<List<ProductOffers>>, ScraperService<List<ProductOffers>>>();

            //strategy pattern
            services.AddScoped<IScrapingStrategy<List<Product>>, SearchPageScraper>();
            services.AddScoped<IScrapingStrategy<Product>, ProductDetailsPageScraper>();
            services.AddScoped<IScrapingStrategy<List<ProductOffers>>, ProductOffersScraper>();

            services.AddScoped<IClientHandler, ClientHandler>();
            // Registers both your custom service and its configured HttpClient
            services.AddHttpClient<IClientHandler, ClientHandler>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                // Automatically handles GZip and Brotli compression
                AutomaticDecompression = System.Net.DecompressionMethods.All
                 //System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
            });

            services.AddTransient<IGenericFieldExtractor, GenericFieldExtractor>();
            return services;
        }
    }
}
