using AmazonScraper.Core.Interfaces;
using AmazonScraper.Infrastructure.Scraper;
using HtmlAgilityPack;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AmazonScraper.Core.Interfaces.IScrapingStrategy;
using Xunit;
using AmazonScraper.Core.Entities;
using FluentAssertions;

namespace AmazonScraper.Tests.Services
{
    public class ScraperServiceTests
    {
        private readonly Mock<IClientHandler> _clientHandlerMock;

        public ScraperServiceTests()
        {
            _clientHandlerMock = new Mock<IClientHandler>();
        }

        [Fact]
        public void Test1()
        {
            Assert.True(true);
        }

    }
}
