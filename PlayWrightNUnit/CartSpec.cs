using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using PlayWrightNUnit.Attributes;
using PlayWrightNUnit.Models;

namespace PlayWrightNUnit
{
    public class CartSpec
    {
        private const string HttpsWwwDemoblazeComCartHtml = @"https://www.demoblaze.com/cart.html";
        private const string HttpsWwwDemoblazeCom = @"https://www.demoblaze.com/";

        protected List<Product> Products = new List<Product>();
        protected IBrowserContext Context;
        private IPage page;
        private IPlaywright playwright;
        private IBrowser browser;

        [OneTimeSetUp]
        public async Task ScenarioSetUp()
        {
            Products = new List<Product>();
            Product product =  new Product("Sony vaio i5",
                790,
                "Sony is so confident that the VAIO S is a superior ultraportable laptop that the company proudly compares the notebook to",
                "Laptops");
            Products.Add(product);
            product = new Product("Apple monitor 24",
                400,
                "LED Cinema Display features a 27-inch glossy LED-backlit TFT active-matrix LCD display with IPS technology and an optimum",
                "Monitors");
            Products.Add(product);

            await LoginAsync();
        }

        [OneTimeTearDown]
        public  async  Task TearDown()
        {
            if (playwright != null) playwright.Dispose();
            if (browser != null) await browser.DisposeAsync();
        }

        private async Task LoginAsync()
        {
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {Headless = false, SlowMo = 50,});
            Context = await browser.NewContextAsync();
            await Context.StorageStateAsync(new BrowserContextStorageStateOptions
            {
                Path = "state.json"
            });
            page = await Context.NewPageAsync();
            await page.GotoAsync(HttpsWwwDemoblazeComCartHtml);
            await page.ClickAsync("a:has-text('Log In')");
            await Helpers.Login("tyschenk@20@gmail.com", "12345678", page);
        }
        
        [Test,
         PlaywrightTest(
             nameof(CartSpec),
             "When items added should display correct items price'")]
        public async Task Cart_WhenAdded_ShouldDisplayCorrectTotalPrice()
        {
            
            page.Dialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Type);
                Assert.AreEqual(string.Empty, e.DefaultValue);
                Assert.AreEqual("Product added", e.Message);
                await e.AcceptAsync();
            };
           
            await AddProductsToCart(page);
            await page.GotoAsync(HttpsWwwDemoblazeComCartHtml);
            await page.ClickAsync("id=totalp");
            string totalPayment = await page.TextContentAsync("id=totalp")??"0";
            var expectedTotal = Products.Select(e=>e.Price).Sum();
            Assert.AreEqual((int)expectedTotal,Int32.Parse(totalPayment));
        }

        private async Task AddProductsToCart(IPage page)
        {
            foreach (var product in Products)
            {
                await page.GotoAsync(HttpsWwwDemoblazeCom);
                await page.ClickAsync($"a:has-text('{product.Category}')");
                await page.ClickAsync($"a:has-text('{product.Name}')");
                await page.ClickAsync("a:has-text('Add to cart')");
                await page.WaitForLoadStateAsync(LoadState.Load);
                await page.WaitForRequestFinishedAsync();
            }
        }


        [Test,
         PlaywrightTest(
             nameof(CartSpec),
             "When tall items is deleted should display zero price in total'")]
        public async Task Cart_DeleteItem_ShouldDisplayTotalAsZero()
        {
            page.Dialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Type);
                Assert.AreEqual(string.Empty, e.DefaultValue);
                Assert.AreEqual("Product added", e.Message);
                await e.AcceptAsync();
            };
            await AddProductsToCart(page);
            await page.GotoAsync(HttpsWwwDemoblazeComCartHtml);
            await ClearShoppingCart(Products, page);
            await page.GotoAsync(HttpsWwwDemoblazeComCartHtml);
            var totalVisible =  await page.IsVisibleAsync("id=totalp");
            Assert.False(totalVisible);
        }
        private async Task ClearShoppingCart(List<Product> products, IPage page)
        {
            for (int i = 0; i < products.Count; i++)
            {
              await page.ClickAsync($"//*[@id=\"tbodyid\"]/tr[{1}]/td[4]/a");
              await page.WaitForRequestFinishedAsync();
            }
        }
    }
}