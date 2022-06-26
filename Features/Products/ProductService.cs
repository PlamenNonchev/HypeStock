using HypeStock.Data;
using HypeStock.Data.Models;
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
    }
}
