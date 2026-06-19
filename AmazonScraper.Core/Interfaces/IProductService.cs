using AmazonScraper.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonScraper.Core.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsbyPageAsync(string? query, int page = 1, bool refresh = false);
        Task<Product> GetProductbyASINAsync(string asin);
    }
}
