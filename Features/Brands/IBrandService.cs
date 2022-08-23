using HypeStock.Features.Brands.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Brands
{
    public interface IBrandService
    {
        public Task<int> Create(string name, string description, string imageUrl);
        public Task<BrandDetailsServiceModel> Get(int brandId);
        public Task<IEnumerable<BrandDetailsServiceModel>> GetHot();
        public Task<BrandDetailsServiceModel> Vote(int brandId, int likes, int dislikes);
    }
}
