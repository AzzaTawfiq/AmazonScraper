using AmazonScraper.Core.Interfaces;
using AmazonScraper.Infrastructure.Scraper;

namespace AmazonScraper.Api.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register your dependencies here
            services.AddScoped<IScraperService, ScraperService>();

            // Always return the services collection to allow method chaining
            return services;
        }
    }
}
