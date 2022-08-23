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
        public decimal Price { get; set; }
        public string Colorway { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime DatePosted { get; set; }
        public decimal LikeRatio
        {
            get
            {
                return Dislikes == 0 ? 100 : (Likes / Dislikes) * 100;
            }
        }

        public IEnumerable<RetailerProduct> ProductRetailers { get; set; }
    }
}
