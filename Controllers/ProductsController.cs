using HypeStock.Data;
using HypeStock.Data.Models;
using HypeStock.Infrastructure;
using HypeStock.Models.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HypeStock.Controllers
{
    public class ProductsController: ApiController
    {
        private readonly HypeStockDbContext data;

        public ProductsController(HypeStockDbContext data)
        {
            this.data = data;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateProductModel model)
        {
            var userId = this.User.GetId();

            var product = new Product
            {
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                BrandId = model.BrandId,
            };

            this.data.Add(product);

            await this.data.SaveChangesAsync();

            return Created(nameof(this.Create), product.Id);
        }
    }
}
