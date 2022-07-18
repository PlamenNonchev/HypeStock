using HypeStock.Features.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Products
{
    public interface IProductService
    {
        public Task<int> Create(string imageUrl, string description, int brandId);
        public Task<bool> Update(int id, string description, string imageUrl);
        public Task<bool> Delete(int id);
        public Task<IEnumerable<ProductDetailsServiceModel>> GetProductsByBrand(int brandId);
        public Task<ProductDetailsServiceModel> Details(int id);
    }
}
