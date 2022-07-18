using HypeStock.Data;
using HypeStock.Data.Models;
using HypeStock.Features.Products.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Products
{
    public class ProductService : IProductService
    {
        private readonly HypeStockDbContext data;

        public ProductService(HypeStockDbContext data)
        {
            this.data = data;
        }
        public async Task<int> Create(string imageUrl, string description, int brandId)
        {
            var product = new Product
            {
                Description = description,
                ImageUrl = imageUrl,
                BrandId = brandId,
            };

            this.data.Add(product);

            await this.data.SaveChangesAsync();

            return product.Id;
        }

        public async Task<bool> Update(int id, string description, string imageUrl)
        {
            var product = await this.data
                .Products
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return false;
            }

            product.Description = description ?? product.Description;
            product.ImageUrl = imageUrl ?? product.ImageUrl;

            await this.data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var product = await this.data
                .Products
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return false;
            }

            this.data.Products.Remove(product);

            await this.data.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ProductDetailsServiceModel>> GetProductsByBrand(int brandId)
            => await this.data
                .Products
                .Where(p => p.BrandId == brandId)
                .Select(p => new ProductDetailsServiceModel
                {
                    Id = p.Id,
                    ImageUrl = p.ImageUrl,
                })
                .ToListAsync();

        public async Task<ProductDetailsServiceModel> Details(int id)
            => await this.data
                .Products.Include(p => p.Brand)
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailsServiceModel
                {
                    Id = p.Id,
                    BrandId = p.BrandId,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description,
                    Brand = p.Brand.Name,
                })
                .FirstOrDefaultAsync();

    }
}
