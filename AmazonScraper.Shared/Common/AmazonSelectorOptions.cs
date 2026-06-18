using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonScraper.Shared.Common
{
    public class AmazonSelectorOptions
    {
        public List<string> Title { get; set; } = [];
        public List<string> Price { get; set; } = [];
        public List<string> Rating { get; set; } = [];
        public List<string> ReviewCount { get; set; } = [];
        public List<string> ProductURL { get; set; } = [];
        public List<string> MainProductImageURL { get; set; } = [];
    }
}
