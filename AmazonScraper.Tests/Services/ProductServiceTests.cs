using AmazonScraper.Application.Services;
using AmazonScraper.Core.Entities;
using AmazonScraper.Core.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AmazonScraper.Tests.Services
{
    public class ProductServiceTests
    {
        //[Fact]
        //public async Task SearchAsync_ValidKeyword_ReturnsProducts()
        //{
        //    // Arrange
        //    var products = new List<Product>
        //{
        //    new Product { Title = "Laptop" }
        //};

        //    var scraperMock = new Mock<IScraperService>();

        //    scraperMock
        //        .Setup(x => x.ScrapeAmazonProductsbyPageAsync("laptop", 1))
        //        .ReturnsAsync(products);

        //    var service = new ProductService();

        //    // Act
        //    var result = await service.GetProductsbyPageAsync("laptop", 1);

        //    // Assert
        //    result.Should().HaveCount(1);
        //    result[0].Title.Should().Contain("Laptop");
        //}
    }
}
