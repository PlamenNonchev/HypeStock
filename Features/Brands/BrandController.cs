using HypeStock.Features.Brands.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Brands
{
    [Authorize]
    public class BrandController: ApiController
    {
        private readonly IBrandService brandService;

        public BrandController(IBrandService brandService)
        {
            this.brandService = brandService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<BrandDetailsServiceModel> GetBrandById(int id)
        {
            return await this.brandService.Get(id);
        }

        [HttpGet]
        [Route("all")]
        public async Task<IEnumerable<BrandDetailsServiceModel>> GetAll()
        {
            return await this.brandService.GetAll();
        }

        [HttpGet]
        [Route("hot")]
        public async Task<IEnumerable<BrandDetailsServiceModel>> GetPopularBrands()
        {
            return await this.brandService.GetHot();
        }

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<ActionResult<int>> Create(CreateBrandServiceModel model)
        {
            var id = await brandService.Create(model.Name, model.ImageUrl, model.Description);

            return Created(nameof(this.Create), id);
        }

        [HttpPut]
        [Route(nameof(Vote))]
        public async Task<ActionResult<BrandDetailsServiceModel>> Vote(BrandDetailsServiceModel brand)
        {
            var updatedBrand = await brandService.Vote(brand.Id, brand.Likes, brand.Dislikes);

            return Ok(updatedBrand);
        }
    }
}
