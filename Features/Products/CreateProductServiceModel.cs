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
    }
}