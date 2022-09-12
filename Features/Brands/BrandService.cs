using HypeStock.Data;
using HypeStock.Data.Models;
using HypeStock.Features.Brands.Models;
using HypeStock.Features.Products.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Brands
{
    public class BrandService : IBrandService
    {
        private readonly HypeStockDbContext data;

        public BrandService(HypeStockDbContext data)
        {
            this.data = data;
        }
        public async Task<BrandDetailsServiceModel> Get(int brandId)
        {
            var brand = await this.data.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == brandId);

            var monthReleases = SetMonthReleases(brand);

            var res = new BrandDetailsServiceModel()
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                ImageUrl = brand.ImageUrl,
                ReleasesByMonth = monthReleases,
                Likes = brand.Likes,
                Dislikes = brand.Dislikes,
                LikeRatio = brand.LikeRatio.ToString("#.##"),
            };

            return res;
        }

        public async Task<IEnumerable<BrandDetailsServiceModel>> GetAll() 
        {
            var brands = await this.data.Brands.Select(b => new BrandDetailsServiceModel
            {
                Id = b.Id,
                Name = b.Name,
            }).ToListAsync();

            return brands;
        }

        public async Task<IEnumerable<BrandDetailsServiceModel>> GetHot()
        {
            var brands = await this.data.Brands.ToListAsync();
            return brands.OrderBy(b => b.LikeRatio)
                .Take(3)
                .Select(b => new BrandDetailsServiceModel
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToList();
        }

        public async Task<int> Create(string name, string description, string imageUrl)
        {
            var brand = new Brand
            { 
                Name = name,
                Description = description,
                ImageUrl = imageUrl,
                Likes = 0,
                Dislikes = 0,
            };

            this.data.Add(brand);

            await this.data.SaveChangesAsync();

            return brand.Id;
        }

        public async Task<BrandDetailsServiceModel> Vote(int brandId, int likes, int dislikes)
        {
            var brand = await this.data.Brands.Include(b => b.Products).FirstOrDefaultAsync(b => b.Id == brandId);

            brand.Likes = likes;
            brand.Dislikes = dislikes;

            await this.data.SaveChangesAsync();

            var monthReleases = SetMonthReleases(brand);

            return new BrandDetailsServiceModel()
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                ImageUrl = brand.ImageUrl,
                ReleasesByMonth = monthReleases,
                Likes = brand.Likes,
                Dislikes = brand.Dislikes,
                LikeRatio = brand.LikeRatio.ToString("#.##"),
            };
        }

        public IEnumerable<MonthReleasesServiceModel> SetMonthReleases(Brand brand)
        {
            var result = new List<MonthReleasesServiceModel>();
            var currentMonth = DateTime.Now.Month;
            var products = brand.Products;
            for (int i = currentMonth; i < currentMonth + 3; i++)
            {
                result.Add(new MonthReleasesServiceModel()
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                    Releases = products.Where(p => p.ReleaseDate.Month == i).Select(p => new ProductDetailsServiceModel
                    { 
                        Id = p.Id,
                        Brand = p.Brand.Name,
                        Model = p.Model,
                        ImageUrl = p.ImageUrl,
                        Likes = p.Likes,
                        Dislikes = p.Dislikes,
                        LikeRatio = p.LikeRatio.ToString("#.##"),
                        Price = p.Price,
                        Colorway = p.Colorway,
                        ReleaseDate = p.ReleaseDate.Month.ToString() + "." + p.ReleaseDate.Day.ToString(),
                    }).ToList(),
                });
            }

            return result;
        }
    }
}
