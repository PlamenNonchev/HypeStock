using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Data.Models
{
    public class Retailer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public IEnumerable<RetailerProduct> RetailerProducts { get; set; }
    }
}
