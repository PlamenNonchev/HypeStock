using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Data.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public int BrandId { get; set; }
        [Required]
        public Brand Brand { get; set; }
        public string Model { get; set; }
    }
}
