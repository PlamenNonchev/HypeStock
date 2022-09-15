using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Data.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        //public decimal LikeRatio
        //{
        //    get
        //    {
        //        if (Likes + Dislikes == 0)
        //        {
        //            return 0;
        //        }

        //        return ((decimal)Likes / (Likes + Dislikes)) * 100;
        //    }
        //}
        public IEnumerable<Product> Products { get; } = new HashSet<Product>();
        public IEnumerable<UserBrandLikes> LikesFromUsers { get; set; }
    }
}
