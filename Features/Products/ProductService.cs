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
        public async Task<int> Create(string imageUrl, string description, int brandId, string model, string colorway, DateTime releaseDate)
        {
            var product = new Product
            {
                Description = description,
                ImageUrl = imageUrl,
                BrandId = brandId,
                Model = model,
                Colorway = colorway,
                ReleaseDate = releaseDate,
                Likes = 0,
                Dislikes = 0,
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

        public async Task<IEnumerable<ProductDetailsServiceModel>> GetHotProducts() 
        {
            var products = await this.data.Products.Include(p => p.Brand).ToListAsync();
            return products.OrderBy(p => p.LikeRatio).Take(4)
                .Select(p => new ProductDetailsServiceModel
                {
                    Id = p.Id,
                    Brand = p.Brand.Name,
                    Model = p.Model,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description,
                })
                .ToList();
        }

        public async Task<IEnumerable<ProductDetailsServiceModel>> GetProductsDroppingShortly()
        {
            var products = await this.data.Products.Include(p => p.Brand).ToListAsync();
            return products.OrderByDescending(p => p.ReleaseDate)
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

        public async Task<ProductDetailsServiceModel> Details(int id)
        {

            var product = await this.data
                .Products.Include(p => p.Brand)
                .Include(p => p.ProductRetailers).ThenInclude(pr => pr.Retailer)
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailsServiceModel
                {
                    Id = p.Id,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description,
                    Brand = p.Brand.Name,
                    Model = p.Model,
                    ReleaseDate = p.ReleaseDate.Date.ToString(),
                    Colorway = p.Colorway,
                    Retailers = p.ProductRetailers.Select(pr => pr.Retailer).ToList(),
                })
                .FirstOrDefaultAsync();

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

            var searchParams = product.Brand + " " + product.Model;
            product.EbayListings = await GetEbayListings(searchParams);
            product.EbaySoldPrices = await GetPastEbaySalePrices(searchParams);

            product.SimilarProducts = similiarProducts;

            return product;
        } 

        public async Task<List<EbayListingServiceModel>> GetEbayListings(string searchKeywords)
        {
            var httpClient = new HttpClient();
            var searchParams = searchKeywords.Split();
            var url = "https://www.ebay.com/sch/i.html?_from=R40&_sacat=0&LH_TitleDesc=0&_nkw=" + String.Join("+", searchParams);
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var productUl = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.HasClass("srp-results"))
                .ToList();

            var listings = productUl[0].Descendants("li")
                .Where(node => node.HasClass("s-item"))
                .Take(4)
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
                    Title = listingInfo.Descendants("h3").FirstOrDefault(node => node.HasClass("s-item__title")).InnerText,
                    ImageUrl = imageSection.Descendants("img").FirstOrDefault().GetAttributeValue("src", ""),
                    Url = imageSection.Descendants("a").FirstOrDefault().GetAttributeValue("href", ""),
                    Condition = listingInfo.Descendants("div").FirstOrDefault(node => node.HasClass("s-item__subtitle")).Descendants("span").FirstOrDefault().InnerText,
                    Price = listingDetails.Descendants("span").FirstOrDefault(node => node.HasClass("s-item__price")).InnerText,
                };

                result.Add(resultListing);
            }

            return result;
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
                result.Add(decimal.Parse(price.Remove(0, 1)));
            }

            return result;
        }
    }
}
