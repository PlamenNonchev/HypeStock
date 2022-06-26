using HypeStock.Data;
using HypeStock.Data.Models;
using HypeStock.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HypeStock.Features.Products
{
    public class ProductsController: ApiController
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateProductModel model)
        {
            var userId = this.User.GetId();

            var id = await productService.Create(model.ImageUrl, model.Description, model.BrandId);

            return Created(nameof(this.Create), id);
        }
    }
}
