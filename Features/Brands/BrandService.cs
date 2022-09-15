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
        public async Task<BrandDetailsServiceModel> Get(int brandId, string userId)
        {
            var brand = await this.data.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == brandId);

            var monthReleases = await SetMonthReleases(brand);
            var currentUserLiked = CheckIfUserHasLiked(brandId, userId);
            var currentUserDisliked = CheckIfUserHasDisliked(brandId, userId);
            var likesCount = await GetLikesCount(brandId);
            var dislikesCount = await GetDislikesCount(brandId);
            var likeRatio = CalculateLikeRatio(likesCount, dislikesCount);

            var res = new BrandDetailsServiceModel()
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                ImageUrl = brand.ImageUrl,
                ReleasesByMonth = monthReleases,
                Likes = likesCount,
                Dislikes = dislikesCount,
                LikeRatio = likeRatio.ToString("#.##"),
                HasUserLiked = currentUserLiked,
                HasUserDisliked = currentUserDisliked,
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
            var brandDtos = new List<BrandDetailsServiceModel>();
            foreach (var brand in brands)
            {
                var brandLikes = await GetLikesCount(brand.Id);
                var brandDislikes = await GetDislikesCount(brand.Id);
                var likeRatio = CalculateLikeRatio(brandLikes, brandDislikes);
                brandDtos.Add(new BrandDetailsServiceModel
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    LikeRatio = likeRatio.ToString("#.##"),
                });
            }
            return brandDtos.OrderByDescending(b => b.LikeRatio)
                .Take(3)
                .ToList();
        }

        public async Task<int> Create(string name, string description, string imageUrl)
        {
            var brand = new Brand
            { 
                Name = name,
                Description = description,
                ImageUrl = imageUrl,
            };

            this.data.Add(brand);

            await this.data.SaveChangesAsync();

            return brand.Id;
        }

        public async Task Like(int brandId, string userId)
        {
            var vote = new UserBrandLikes
            {
                UserId = userId,
                BrandId = brandId,
                Type = "Like"
            };

            this.data.Add(vote);

            await this.data.SaveChangesAsync();
        }

        public async Task Dislike(int brandId, string userId)
        {
            var vote = new UserBrandLikes
            {
                UserId = userId,
                BrandId = brandId,
                Type = "Dislike"
            };

            this.data.Add(vote);

            await this.data.SaveChangesAsync();
        }

        public async Task<IEnumerable<MonthReleasesServiceModel>> SetMonthReleases(Brand brand)
        {
            var result = new List<MonthReleasesServiceModel>();
            var currentMonth = DateTime.Now.Month;
            var products = brand.Products;
            for (int i = currentMonth; i < currentMonth + 3; i++)
            {
                var releases = products.Where(p => p.ReleaseDate.Month == i).ToList();
                var releasesDtos = new List<ProductDetailsServiceModel>();
                foreach (var release in releases)
                {
                    var likes = await GetProductLikesCount(release.Id);
                    var dislikes = await GetProductDislikesCount(release.Id);
                    var likeRatio = CalculateLikeRatio(likes, dislikes);

                    releasesDtos.Add(new ProductDetailsServiceModel
                    {
                        Id = release.Id,
                        Brand = release.Brand.Name,
                        Model = release.Model,
                        ImageUrl = release.ImageUrl,
                        Likes = likes,
                        Dislikes = dislikes,
                        LikeRatio = likeRatio.ToString("#.##"),
                        Price = release.Price,
                        Colorway = release.Colorway,
                        ReleaseDate = release.ReleaseDate.Month.ToString() + "." + release.ReleaseDate.Day.ToString(),
                    });
                }
                result.Add(new MonthReleasesServiceModel()
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                    Releases = releasesDtos,
                });
            }

            return result;
        }

        private bool CheckIfUserHasLiked(int brandId, string userId)
        {
            return this.data.UserBrandLikes.Any(ub => ub.BrandId == brandId && ub.UserId == userId && ub.Type == "Like");
        }

        private bool CheckIfUserHasDisliked(int brandId, string userId)
        {
            return this.data.UserBrandLikes.Any(ub => ub.BrandId == brandId && ub.UserId == userId && ub.Type == "Dislike");
        }

        private async Task<int> GetLikesCount(int brandId)
        {
            return await this.data.UserBrandLikes.Where(ub => ub.BrandId == brandId && ub.Type == "Like").CountAsync();
        }

        private async Task<int> GetDislikesCount(int brandId)
        {
            return await this.data.UserBrandLikes.Where(ub => ub.BrandId == brandId && ub.Type == "Dislike").CountAsync();
        }

        private decimal CalculateLikeRatio(int likes, int dislikes)
        {
            if (likes + dislikes == 0)
            {
                  return 0;
            }

            return ((decimal)likes / (likes + dislikes)) * 100;
        }

        private async Task<int> GetProductLikesCount(int productId)
        {
            return await this.data.UserProductLikes.Where(up => up.ProductId == productId && up.Type == "Like").CountAsync();
        }

        private async Task<int> GetProductDislikesCount(int productId)
        {
            return await this.data.UserProductLikes.Where(up => up.ProductId == productId && up.Type == "Dislike").CountAsync();
        }
    }
}
