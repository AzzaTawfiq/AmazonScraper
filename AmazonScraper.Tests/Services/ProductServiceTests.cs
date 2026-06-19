using AmazonScraper.Application.Services;
using AmazonScraper.Core.Entities;
using AmazonScraper.Core.Interfaces;
using AmazonScraper.Infrastructure.Scraper;
using FluentAssertions;
using HtmlAgilityPack;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static AmazonScraper.Core.Interfaces.IScrapingStrategy;

namespace AmazonScraper.Tests.Services
{
    public class ProductServiceTests
    {
        //[Fact]
        //public async Task ScrapeDataAsync_ValidKeyword_ReturnsProducts()
        //{
        //    // Arrange
        //    var products = new List<Product>
        //{
        //    new Product { Title = "Laptop" }
        //};

        //    var scraperMock = new Mock<IClientHandler>();

        //    scraperMock
        //        .Setup(x => x.ScrapeAmazonProductsbyPageAsync("laptop", 1))
        //        .ReturnsAsync(products);

        //    var service = new ScraperService<List<Product>>();

        //    // Act
        //    var result = await service.GetProductsbyPageAsync("laptop", 1);

        //    // Assert
        //    result.Should().HaveCount(1);
        //    result[0].Title.Should().Contain("Laptop");
        //}

        //[Fact]
        //public void ScrapeDataAsync_ValidSearchPage_ShouldExtractProducts()
        //{
        //    // Arrange
        //    var document = LoadFixture("SearchforWirelessHeadphones.html");

        //    var scraper = new ScraperService<List<Product>>();

        //    // Act
        //    var products = scraper.ScrapeDataAsync(document, "SearchPageScraper");

        //    // Assert
        //    products.Should().NotBeNull();

        //    //products.First().Asin.Should().Be("B0ABC123");
        //    //products.First().Title.Should().Be("Dell Laptop");
        //}

        //[Fact]
        //public async Task ScrapeDataAsync_ValidData_ShouldReturnProducts()
        //{
        //    var clientHandler = new Mock<IClientHandler>();

        //    clientHandler
        //        .Setup(x => x.GetHTMLData(It.IsAny<string>()))
        //        .ReturnsAsync(
        //            File.ReadAllText("Fixtures/SearchforWirelessHeadphones.html"));

        //    var scraper = new ScraperService<List<Product>>(
        //        clientHandler.Object, null);
        //    var result =
        //        await scraper.ScrapeDataAsync("", "SearchPageScraper");

        //    result.Should().NotBeEmpty();
        //}

        private HtmlDocument LoadFixture(string fileName)
        {
            var html = File.ReadAllText(
                Path.Combine("Fixtures", fileName));

            var document = new HtmlDocument();
            document.LoadHtml(html);

            return document;
        }
    }
}
