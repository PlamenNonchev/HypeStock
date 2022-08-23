using HypeStock.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Brands.Models
{
    public class BrandDetailsServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<MonthReleasesServiceModel> ReleasesByMonth { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string LikeRatio { get; set; }
    }
}
