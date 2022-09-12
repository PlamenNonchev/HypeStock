using System;
using System.ComponentModel.DataAnnotations;

namespace HypeStock.Features.Products
{
    public class CreateProductServiceModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public int BrandId { get; set; }
        public string Model { get; set; }
        public string Colorway { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}