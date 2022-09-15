using HypeStock.Data.Models;
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
        public string Model { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string LikeRatio { get; set; }
        public string ReleaseDate { get; set; }
        public bool Released { get; set; }
        public string Colorway { get; set; }
        public decimal Price { get; set; }
        public string EbayUrl { get; set; }
        public bool HasUserLiked { get; set; }
        public bool HasUserDisliked { get; set; }
        public List<ProductDetailsServiceModel> SimilarProducts { get; set; }
        public List<RetailerServiceModel> Retailers { get; set; }
        public List<EbayListingServiceModel> EbayListings { get; set; }
        public List<decimal> EbaySoldPrices { get; set; }
    }
}
