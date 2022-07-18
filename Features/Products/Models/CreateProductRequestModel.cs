using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Products.Models
{
    public class CreateProductRequestModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public int BrandId { get; set; }
    }
}
