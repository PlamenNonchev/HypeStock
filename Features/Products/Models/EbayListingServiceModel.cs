using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Products.Models
{
    public class EbayListingServiceModel
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Condition { get; set; } //TODO: Change to enum
        public string Price { get; set; }
        public string Url { get; set; }
    }
}
