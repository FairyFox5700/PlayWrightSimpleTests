using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using PlayWrightNUnit.Attributes;
using PlayWrightNUnit.Models;
using PlayWrightNUnit.PageObjects;

namespace PlayWrightNUnit
{
    public class CartSpec
    {
        protected IBrowserContext Context;
        private IPlaywright playwright;
        private IBrowser browser;
        private List<Product> Products;
        
        [OneTimeSetUp]
        public async Task SetUp()
        {
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {Headless = false, SlowMo = 50,});
        }
        
        [Test,
         PlaywrightTest(
             nameof(CartSpec),
             "When tall items is deleted should display zero price in total'")]
        public async Task Cart_DeleteItem_ShouldDisplayTotalAsZero()
        {
            var page = new CartPageObject(await browser.NewPageAsync());
            page.OnDialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Type);
                Assert.AreEqual(string.Empty, e.DefaultValue);
                Assert.AreEqual("Product added", e.Message);
                await e.AcceptAsync();
            };
            await page.AddProductsToCart();
            await page.GotoAsync();
            await page.ClearShoppingCart();
            await page.GotoAsync();
            var totalVisible =  await page.GetVisibility("id=totalp");
            Assert.False(totalVisible);
        }
        
        [Test,
         PlaywrightTest(
             nameof(CartSpec),
             "When items added should display correct items price'")]
        public async Task Cart_WhenAdded_ShouldDisplayCorrectTotalPrice()
        {
            var page = new CartPageObject(await browser.NewPageAsync());
            page.OnDialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Type);
                Assert.AreEqual(string.Empty, e.DefaultValue);
                Assert.AreEqual("Product added", e.Message);
                await e.AcceptAsync();
            };
            await page.AddProductsToCart();
            await page.GotoAsync();
            await page.GetTotalPayment();
            var totalPayment = await page.GetTotalPayment();
            var expectedTotal = (await page.GetProducts()).Select(e=>e.Price).Sum();
            Assert.AreEqual((int)expectedTotal,Int32.Parse(totalPayment));
        }
        
    }
}