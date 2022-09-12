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

    using static Infrastructure.WebConstants;

    [Authorize]
    public class ProductsController: ApiController
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }


        [HttpGet]
        [Route(nameof(GetProductsByBrand))]
        public async Task<IEnumerable<ProductDetailsServiceModel>> GetProductsByBrand(int brandId)
        {
            return await this.productService.GetProductsByBrand(brandId);
        }

        [HttpGet]
        [Route("all")]
        public async Task<IEnumerable<ProductDetailsServiceModel>> GetAllProducts()
        {
            return await this.productService.GetAll();
        }

        [HttpGet]
        [Route(Id)]
        public async Task<ActionResult<ProductDetailsServiceModel>> Details(int id)
            => await this.productService.Details(id);

        [HttpGet]
        [Route("hot")]
        public async Task<IEnumerable<ProductDetailsServiceModel>> GetHotProducts()
        {
            return await this.productService.GetHotProducts();
        }

        [HttpGet]
        [Route("soon")]
        public async Task<IEnumerable<ProductDetailsServiceModel>> GetProductsDroppingShortly()
        {
            return await this.productService.GetProductsDroppingShortly();
        }

        [HttpGet]
        [Route("justAnnounced")]
        public async Task<IEnumerable<ProductDetailsServiceModel>> GetJustAnnouncedProducts()
        {
            return await this.productService.GetJustAnnouncedProducts();
        }

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<ActionResult<int>> Create(CreateProductServiceModel model)
        {
            var userId = this.User.GetId();

            var id = await productService.Create(model.ImageUrl, model.Description, model.BrandId, model.Model, model.Colorway, model.Price, model.ReleaseDate);

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

        [HttpDelete]
        [Route(Id)]
        public async Task<ActionResult> Delete(int id)
        {
            //Add functionality to check if the user has rights to edit products.
            var deleted = await this.productService.Delete(id);

            if (!deleted)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPut]
        [Route("pick")]
        public async Task<ActionResult> PickProducts(UpdateEditorsPicksModel model)
        {
            var updated = await this.productService.UpdateEditorsPicks(model.MainProductId, model.SideProductId);
            return Ok();
        }
    }
}
