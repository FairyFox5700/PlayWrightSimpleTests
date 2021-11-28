using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlayWrightNUnit.Models;

namespace PlayWrightNUnit.PageObjects
{
    public class CartPageObject
    {
        private readonly IPage _page;
        private const string HttpsWwwDemoblazeComCartHtml = @"https://www.demoblaze.com/cart.html";
        private const string HttpsWwwDemoblazeCom = @"https://www.demoblaze.com/";
        
        public CartPageObject(IPage page)
        {
            _page = page;
        }

        public EventHandler<IDialog> OnDialog;

        public async Task GotoAsync()
        {
            await _page.GotoAsync(HttpsWwwDemoblazeComCartHtml);
        }

        public async Task<List<Product>> GetProducts()
        {
            var products = new List<Product>();
            Product product =  new Product("Sony vaio i5",
                790,
                "Sony is so confident that the VAIO S is a superior " +
                "ultraportable laptop that the company proudly compares the notebook to",
                "Laptops");
            products.Add(product);
            product = new Product("Apple monitor 24",
                400,
                "LED Cinema Display features a 27-inch glossy " +
                "LED-backlit TFT active-matrix LCD display with IPS technology and an optimum",
                "Monitors");
            products.Add(product);
            return products;
        }
        
        public async Task ClearShoppingCart()
        {
            for (int i = 0; i < (await GetProducts()).Count; i++)
            {
                await _page.ClickAsync($"//*[@id=\"tbodyid\"]/tr[{1}]/td[4]/a");
                await _page.WaitForRequestFinishedAsync();
            }
        }

        public async Task AddProductsToCart()
        {
            foreach (var product in await GetProducts())
            {
                await _page.GotoAsync(HttpsWwwDemoblazeCom);
                await _page.ClickAsync($"a:has-text('{product.Category}')");
                await _page.ClickAsync($"a:has-text('{product.Name}')");
                await _page.ClickAsync("a:has-text('Add to cart')");
                await _page.WaitForLoadStateAsync(LoadState.Load);
                await _page.WaitForRequestFinishedAsync();
            }
        }
        public async Task<bool?> GetVisibility(string field)
        {
            return await _page.IsVisibleAsync(field);
        }
        public async Task<string> GetTotalPayment()
        {
            await _page.ClickAsync("id=totalp");
            string totalPayment = await _page.TextContentAsync("id=totalp")??"0";
            return totalPayment;
        }
    }
}