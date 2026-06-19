using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonScraper.Shared.Common
{
    public class AmazonURL
    {
        public string AmazonBaseURL { get; set; } = string.Empty;
        public string SearchURL { get; set; } = string.Empty;
        public string ProductDetailsURL { get; set; } = string.Empty;
        public string ProductOffersURL { get; set; } = string.Empty;
    }
}
