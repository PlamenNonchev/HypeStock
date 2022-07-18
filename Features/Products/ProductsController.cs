using HypeStock.Data;
using HypeStock.Data.Models;
using HypeStock.Features.Products.Models;
using HypeStock.Infrastructure;
using HypeStock.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HypeStock.Features.Products
{
    [Authorize]
    public class ProductsController: ApiController
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }


        [HttpGet]
        [Route(nameof(GetProductsByUser))]
        public async Task<IEnumerable<ProductDetailsServiceModel>> GetProductsByUser(int brandId)
        {
            return await this.productService.GetProductsByBrand(brandId);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ProductDetailsServiceModel>> Details(int id)
            => await this.productService.Details(id);

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<ActionResult<int>> Create(CreateProductServiceModel model)
        {
            var userId = this.User.GetId();

            var id = await productService.Create(model.ImageUrl, model.Description, model.BrandId);

            return Created(nameof(this.Create), id);
        }

        [HttpPut]
        public async Task<ActionResult> Update(UpdateProductRequestModel model)
        {
            //Add functionality to check if the user has rights to edit products.
            var updated = await this.productService.Update(
                model.Id,
                model.Description,
                model.ImageUrl);

            if (!updated)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
