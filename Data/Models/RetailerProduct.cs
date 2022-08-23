using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Data.Models
{
    public class RetailerProduct
    {
        public int RetailerId { get; set; }
        public Retailer Retailer { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
