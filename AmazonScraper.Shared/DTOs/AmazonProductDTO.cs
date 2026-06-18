using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonScraper.Shared.DTOs
{
    public class AmazonProductDTO
    {
        public string ASIN { get; set; } = string.Empty; //Amazon Standard Identification Number
        public string ProductTitle { get; set; } = string.Empty;
        public string CurrentPrice { get; set; } = string.Empty;
        public string OriginalPrice { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Rating { get; set; } = string.Empty; //Star rating(out of 5)
        public string TotalReviews { get; set; } = string.Empty;
        public string MainProductImageURL { get; set; } = string.Empty;
        public string IsPrime { get; set; } = string.Empty; //Prime eligibility
        public string ProductURL { get; set; } = string.Empty;
    }
}
