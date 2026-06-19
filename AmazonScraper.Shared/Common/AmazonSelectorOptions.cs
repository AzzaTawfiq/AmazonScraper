using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonScraper.Shared.Common
{
    public class AmazonSelectorOptions
    {
        public string DataComponent { get; set; } = string.Empty;
        public List<string> ASIN { get; set; } = [];
        public List<string> Title { get; set; } = [];
        public List<string> Price { get; set; } = [];
        public List<string> Rating { get; set; } = [];
        public List<string> ReviewCount { get; set; } = [];
        public List<string> ProductURL { get; set; } = [];
        public List<string> MainProductImageURL { get; set; } = [];
        public List<string> IsPrime { get; set; } = [];

        public List<string> FeatureBullets { get; set; } = [];
        public List<string> AllProductImages { get; set; } = [];
        public List<string> ProductSpecifications { get; set; } = [];
        public List<string> Variants { get; set; } = [];
        public List<string> Categories { get; set; } = [];
        public List<string> brand { get; set; } = [];
        public List<string> Availability { get; set; } = [];
        public List<string> Description { get; set; } = [];

        public string OfferDataComponent { get; set; } = string.Empty;

    }
}
