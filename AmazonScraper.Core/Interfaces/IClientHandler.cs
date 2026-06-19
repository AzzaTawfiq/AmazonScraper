using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonScraper.Core.Interfaces
{
    public interface IClientHandler
    {
        Task<string> GetHTMLData(string url);
    }
}
