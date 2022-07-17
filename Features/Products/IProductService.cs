﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Products
{
    public interface IProductService
    {
        public Task<int> Create(string imageUrl, string description, int brandId);
        public Task<IEnumerable<ProductDetailsResponseModel>> GetProductsByBrand(int brandId);
    }
}
