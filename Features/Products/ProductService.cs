using HtmlAgilityPack;
using HypeStock.Data;
using HypeStock.Data.Models;
using HypeStock.Features.Products.Models;
using Microsoft.EntityFrameworkCore;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HypeStock.Features.Products
{
    public class ProductService : IProductService
    {
        private readonly HypeStockDbContext data;
        static ScrapingBrowser _ScrapingBrowser = new ScrapingBrowser();

        public ProductService(HypeStockDbContext data)
        {
            this.data = data;
        }
        public async Task<int> Create(string imageUrl, string description, int brandId, string model, string colorway, decimal price, DateTime releaseDate)
        {
            var product = new Product
            {
                Description = description,
                ImageUrl = imageUrl,
                BrandId = brandId,
                Model = model,
                Colorway = colorway,
                ReleaseDate = releaseDate,
                Price = price,
            };

            this.data.Add(product);

            await this.data.SaveChangesAsync();

            return product.Id;
        }

        public async Task<bool> Update(int id, string description, string imageUrl)
        {
            var product = await this.data
                .Products
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return false;
            }

            product.Description = description ?? product.Description;
            product.ImageUrl = imageUrl ?? product.ImageUrl;

            await this.data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var product = await this.data
                .Products
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return false;
            }

            this.data.Products.Remove(product);

            await this.data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateEditorsPicks(int mainProductId, int sideProductId)
        {
            var editorsPicks = await this.data.EditorsPicks.ToListAsync();
            var mainProduct = editorsPicks.First();
            var sideProduct = editorsPicks.Last();
            mainProduct.ProductId = mainProductId;
            sideProduct.ProductId = sideProductId;

            await this.data.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDetailsServiceModel>> GetProductsByBrand(int brandId)
            => await this.data
                .Products
                .Where(p => p.BrandId == brandId)
                .Select(p => new ProductDetailsServiceModel
                {
                    Id = p.Id,
                    ImageUrl = p.ImageUrl,
                })
                .ToListAsync();

        public async Task<IEnumerable<ProductDetailsServiceModel>> GetAll()
            => await this.data.Products
                .Select(p => new ProductDetailsServiceModel
                {
                    Id = p.Id,
                    Brand = p.Brand.Name,
                    Model = p.Model,
                }).ToListAsync();

        public async Task<IEnumerable<ProductDetailsServiceModel>> GetHotProducts() 
        {
            var products = await this.data.Products.Include(p => p.Brand).ToListAsync();
            var productDtos = new List<ProductDetailsServiceModel>();
            foreach (var product in products)
            {
                var likes = await GetLikesCount(product.Id);
                var dislikes = await GetDislikesCount(product.Id);
                var likeRatio = CalculateLikeRatio(likes, dislikes);
                productDtos.Add(new ProductDetailsServiceModel()
                {
                    Id = product.Id,
                    Brand = product.Brand.Name,
                    Model = product.Model,
                    ImageUrl = product.ImageUrl,
                    Description = product.Description,
                    LikeRatio = likeRatio.ToString("#.##"),
                });
            }

            return productDtos.OrderByDescending(p => p.LikeRatio).Take(4).ToList();
        }

        public async Task<IEnumerable<ProductDetailsServiceModel>> GetProductsDroppingShortly()
        {
            var products = await this.data.Products.Include(p => p.Brand).ToListAsync();
            return products.Where(p => p.ReleaseDate > DateTime.Now).OrderBy(p => p.ReleaseDate)
                .Take(4)
                .Select(p => new ProductDetailsServiceModel
                {
                    Id = p.Id,
                    Brand = p.Brand.Name,
                    Model = p.Model,
                    ImageUrl = p.ImageUrl,
                })
                .ToList();
        }

        public async Task<IEnumerable<ProductDetailsServiceModel>> GetJustAnnouncedProducts()
        {
            var products = await this.data.Products.Include(p => p.Brand).ToListAsync();
            return products.OrderByDescending(p => p.DatePosted)
                .Take(4)
                .Select(p => new ProductDetailsServiceModel
                {
                    Id = p.Id,
                    Brand = p.Brand.Name,
                    Model = p.Model,
                    ImageUrl = p.ImageUrl,
                })
                .ToList();
        }

        public async Task<ProductDetailsServiceModel> Details(int id, string userId)
        {
            var product = await this.data
                .Products.Include(p => p.Brand)
                .Include(p => p.ProductRetailers).ThenInclude(pr => pr.Retailer)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            var hasUserLiked = CheckIfUserHasLiked(id, userId);
            var hasUserDisliked = CheckIfUserHasDisliked(id, userId);
            var likesCount = await GetLikesCount(id);
            var dislikesCount = await GetDislikesCount(id);
            var likeRatio = CalculateLikeRatio(likesCount, dislikesCount);

            var result = new ProductDetailsServiceModel
               {
                   Id = product.Id,
                   ImageUrl = product.ImageUrl,
                   Description = product.Description,
                   Brand = product.Brand.Name,
                   Model = product.Model,
                   Likes = likesCount,
                   Dislikes = dislikesCount,
                   LikeRatio = likeRatio.ToString("#.##"),
                   ReleaseDate = product.ReleaseDate.ToShortDateString(),
                   Released = product.ReleaseDate < DateTime.Now,
                   Colorway = product.Colorway,
                   HasUserLiked = hasUserLiked,
                   HasUserDisliked = hasUserDisliked,
               };

            var retailers = product.ProductRetailers.Select(pr => pr.Retailer).Select(r => new RetailerServiceModel
            {
                Id = r.Id,
                Description = r.Description,
                Name = r.Name,
                ImageUrl = r.ImageUrl,
                WebsiteUrl = r.WebsiteUrl,
            }).ToList();

            result.Retailers = retailers;

            var similiarProducts = await this.data
                .Products.Include(p => p.Brand)
                .Where(p => p.Colorway == product.Colorway && p.Id != product.Id)
                .Select(p => new ProductDetailsServiceModel
                {
                    Id = p.Id,
                    ImageUrl = p.ImageUrl,
                    Brand = p.Brand.Name,
                    Model = p.Model,
                })
                .ToListAsync();

            var searchParams = result.Brand + " " + result.Model;
            await AddEbayListings(searchParams, result);
            result.EbaySoldPrices = await GetPastEbaySalePrices(searchParams);

            result.SimilarProducts = similiarProducts;

            return result;
        }

        public async Task<IEnumerable<ProductDetailsServiceModel>> GetLikedByUser(string userId)
        {
            var likedProducts = await this.data.UserProductLikes.Include(up => up.Product).ThenInclude(p => p.Brand)
                .Where(up => up.UserId == userId && up.Type == "Like").ToListAsync();
            var productsDtos = new List<ProductDetailsServiceModel>();
            foreach (var likedProduct in likedProducts)
            {
                var productLikes = await GetProductLikesCount(likedProduct.ProductId);
                productsDtos.Add(new ProductDetailsServiceModel
                {
                    Id = likedProduct.ProductId,
                    Brand = likedProduct.Product.Brand.Name,
                    Model = likedProduct.Product.Model,
                    Price = likedProduct.Product.Price,
                    ReleaseDate = likedProduct.Product.ReleaseDate.Date.ToShortDateString(),
                    Colorway = likedProduct.Product.Colorway,
                    Likes = productLikes,
                    ImageUrl = likedProduct.Product.ImageUrl,
                });
            }

            return productsDtos;
        }

        public async Task Like(int productId, string userId)
        {
            var vote = new UserProductLikes
            {
                UserId = userId,
                ProductId = productId,
                Type = "Like"
            };

            this.data.Add(vote);

            await this.data.SaveChangesAsync();
        }

        public async Task Dislike(int productId, string userId)
        {
            var vote = new UserBrandLikes
            {
                UserId = userId,
                BrandId = productId,
                Type = "Dislike"
            };

            this.data.Add(vote);

            await this.data.SaveChangesAsync();
        }

        public async Task AddEbayListings(string searchKeywords, ProductDetailsServiceModel productResult)
        {
            var httpClient = new HttpClient();
            var searchParams = searchKeywords.Split();
            var url = "https://www.ebay.com/sch/i.html?_from=R40&_sacat=0&LH_TitleDesc=0&_nkw=" + String.Join("+", searchParams);
            productResult.EbayUrl = url;
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var productUl = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.HasClass("srp-results"))
                .ToList();

            var listings = productUl[0].Descendants("li")
                .Where(node => node.HasClass("s-item"))
                .Take(3)
                .ToList();

            var result = new List<EbayListingServiceModel>();

            foreach (var listing in listings)
            {
                var imageSection = listing.Descendants("div")
                    .FirstOrDefault(node => node.HasClass("s-item__image"));

                var listingInfo = listing.Descendants("div")
                    .FirstOrDefault(node => node.HasClass("s-item__info"));

                var listingDetails = listing.Descendants("div")
                   .FirstOrDefault(node => node.HasClass("s-item__details"));

                var resultListing = new EbayListingServiceModel()
                {
                    Title = listingInfo.Descendants("div").FirstOrDefault(node => node.HasClass("s-item__title")).InnerText,
                    ImageUrl = imageSection.Descendants("img").FirstOrDefault().GetAttributeValue("src", ""),
                    Url = imageSection.Descendants("a").FirstOrDefault().GetAttributeValue("href", ""),
                    Condition = listingInfo.Descendants("div").FirstOrDefault(node => node.HasClass("s-item__subtitle")).Descendants("span").FirstOrDefault().InnerText,
                    Price = listingDetails.Descendants("span").FirstOrDefault(node => node.HasClass("s-item__price")).InnerText,
                };

                result.Add(resultListing);
            }

            productResult.EbayListings = result;
        }

        public async Task<List<decimal>> GetPastEbaySalePrices(string searchKeywords)
        {
            var httpClient = new HttpClient();
            var searchParams = searchKeywords.Split();
            var url = "https://www.ebay.com/sch/i.html?_from=R40&_sacat=0&LH_TitleDesc=0&_nkw=" + String.Join("+", searchParams) + "&LH_Sold=1&LH_Complete=1";
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var productUl = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.HasClass("srp-results"))
                .ToList();

            var listings = productUl[0].Descendants("li")
                .Where(node => node.HasClass("s-item") || node.HasClass("srp-river-answer--REWRITE_START"))
                .Take(30)
                .ToList();

            var filteredListings = new List<HtmlNode>();
            foreach (var item in listings)
            {
                if (item.HasClass("srp-river-answer"))
                {
                    break;
                }

                filteredListings.Add(item);
            }

            var result = new List<decimal>();

            foreach (var listing in filteredListings)
            {
                var listingDetails = listing.Descendants("div")
                   .FirstOrDefault(node => node.HasClass("s-item__details"));

                var price = listingDetails.Descendants("span").FirstOrDefault(node => node.HasClass("s-item__price")).InnerText;
                if (!price.Contains("to"))
                {
                    result.Add(decimal.Parse(price.Remove(0, 1)));
                }
            }

            return result;
        }

        private async Task<int> GetProductLikesCount(int productId)
        {
            return await this.data.UserProductLikes.Where(up => up.ProductId == productId && up.Type == "Like").CountAsync();
        }

        private bool CheckIfUserHasLiked(int productId, string userId)
        {
            return this.data.UserProductLikes.Any(up => up.ProductId == productId && up.UserId == userId && up.Type == "Like");
        }

        private bool CheckIfUserHasDisliked(int productId, string userId)
        {
            return this.data.UserProductLikes.Any(up => up.ProductId == productId && up.UserId == userId && up.Type == "Dislike");
        }

        private async Task<int> GetLikesCount(int productId)
        {
            return await this.data.UserProductLikes.Where(up => up.ProductId == productId && up.Type == "Like").CountAsync();
        }

        private async Task<int> GetDislikesCount(int productId)
        {
            return await this.data.UserProductLikes.Where(up => up.ProductId == productId && up.Type == "Dislike").CountAsync();
        }

        private decimal CalculateLikeRatio(int likes, int dislikes)
        {
            if (likes + dislikes == 0)
            {
                return 0;
            }

            return ((decimal)likes / (likes + dislikes)) * 100;
        }
    }
}
