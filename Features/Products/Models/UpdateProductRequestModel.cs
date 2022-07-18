using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Products.Models
{
    public class UpdateProductRequestModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
