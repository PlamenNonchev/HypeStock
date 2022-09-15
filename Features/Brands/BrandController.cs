using HypeStock.Features.Brands.Models;
using HypeStock.Infrastructure.Extensions;
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
            var userId = this.User.GetId();
            return await this.brandService.Get(id, userId);
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
        [Route(nameof(Like))]
        public async Task<ActionResult> Like([FromBody] int brandId)
        {
            var userId = this.User.GetId();
            await brandService.Like(brandId, userId);

            return Ok();
        }

        [HttpPut]
        [Route(nameof(Dislike))]
        public async Task<ActionResult> Dislike([FromBody] int brandId)
        {
            var userId = this.User.GetId();
            await brandService.Dislike(brandId, userId);

            return Ok();
        }
    }
}
