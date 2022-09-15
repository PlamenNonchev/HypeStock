using HypeStock.Features.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Products
{
    public interface IProductService
    {
        public Task<int> Create(string imageUrl, string description, int brandId, string model, string colorway, decimal price, DateTime releaseDate);
        public Task<bool> Update(int id, string description, string imageUrl);
        public Task<bool> Delete(int id);
        public Task<bool> UpdateEditorsPicks(int mainProductId, int sideProductId);
        public Task<IEnumerable<ProductDetailsServiceModel>> GetProductsByBrand(int brandId);
        public Task<IEnumerable<ProductDetailsServiceModel>> GetAll();
        public Task<IEnumerable<ProductDetailsServiceModel>> GetHotProducts();
        public Task<IEnumerable<ProductDetailsServiceModel>> GetProductsDroppingShortly();
        public Task<IEnumerable<ProductDetailsServiceModel>> GetJustAnnouncedProducts();
        public Task<IEnumerable<ProductDetailsServiceModel>> GetLikedByUser(string userId);
        public Task<ProductDetailsServiceModel> Details(int id, string userId);
        public Task Like(int productId, string userId);
        public Task Dislike(int productId, string userId);

        public Task<EditorsPicksServiceModel> GetEditorsPicks();

    }
}
