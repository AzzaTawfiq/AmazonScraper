using AmazonScraper.Core.Entities;
using AmazonScraper.Core.Interfaces;
using AmazonScraper.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AmazonScraper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<List<Product>> Get()
        {
            return await _productService.GetProductsbyPageAsync("all");
        }

        [HttpGet("search")]
        public async Task<List<Product>> Search([FromQuery] string? query, [FromQuery] int page = 1, [FromQuery] bool refresh = false)
        {
            return await _productService.GetProductsbyPageAsync(query, page, refresh);
        }

        [HttpGet("getProductbyASIN")]
        public async Task<Product> GetProductbyPageAsync([FromQuery] string asin)
        {
            return await _productService.GetProductbyASINAsync(asin);
        }

        [HttpGet("{asin}/offers")]
        public async Task<List<ProductOffers>> GetProductOffersAsync(string asin)
        {
            return await _productService.GetProductOffersAsync(asin);
        }
    }
}
