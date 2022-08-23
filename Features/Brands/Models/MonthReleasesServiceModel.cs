using HypeStock.Data.Models;
using HypeStock.Features.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Brands.Models
{
    public class MonthReleasesServiceModel
    {
        public string Month { get; set; }
        public IEnumerable<ProductDetailsServiceModel> Releases { get; set; }
    }
}
