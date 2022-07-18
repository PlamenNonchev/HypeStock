using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Products.Models
{
    public class ProductDetailsServiceModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int BrandId  { get; set; }
        public string Brand { get; set; }
    }
}
