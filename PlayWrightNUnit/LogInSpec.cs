using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using PlayWrightNUnit.Attributes;

namespace PlayWrightNUnit
{
    [TestFixture]
    public class LoginSpec
    {
        private const string HttpsWwwDemoblazeCom = @"https://www.demoblaze.com/";
        private const string AHasTextLogIn = "a:has-text('Log In')";

        [Test,
         PlaywrightTest(
             nameof(LoginSpec),
             "When empty login and password should respond with alert that contains text " +
             "'Please fill out Username and Password.'")]
        public async Task Login_WithEmptyUserNameAndPassword_ShouldAlertMessage()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {Headless = false, SlowMo = 50,});
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();
            page.Dialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Type);
                Assert.AreEqual(string.Empty, e.DefaultValue);
                Assert.AreEqual("Please fill out Username and Password.", e.Message);
                await e.AcceptAsync();
            };
            await page.GotoAsync(HttpsWwwDemoblazeCom);
            await page.ClickAsync(AHasTextLogIn);

            await Helpers.Login("", "", page);
        }

        [Test,
         PlaywrightTest(
             nameof(LoginSpec),
             "When types unexisting email 'User does not exist.'")]
        public async Task Login_WithUnExistingEmail_ShouldAlertMessage()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {Headless = false, SlowMo = 50,});
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();
            page.Dialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Type);
                Assert.AreEqual(string.Empty, e.DefaultValue);
                Assert.AreEqual("User does not exist.", e.Message);
                await e.AcceptAsync();
            };
            await page.GotoAsync(HttpsWwwDemoblazeCom);
            await page.ClickAsync(AHasTextLogIn);
            await Helpers.Login("t@ggg.com", "1234567", page);
        }


        [Test,
         PlaywrightTest(nameof(LoginSpec), "When types invalid password 'User does not exist.'")]
        public async Task Login_WithUnInvalidPassword_ShouldAlertMessage()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {Headless = false, SlowMo = 50,});
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();
            page.Dialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Type);
                Assert.AreEqual(string.Empty, e.DefaultValue);
                Assert.AreEqual("Wrong password.", e.Message);
                await e.AcceptAsync();
            };
            await page.GotoAsync(HttpsWwwDemoblazeCom);
            await page.ClickAsync(AHasTextLogIn);
            await Helpers.Login("t@gmail.com", "123456", page);
        }

        [Test,
         PlaywrightTest(nameof(LoginSpec), "When types valid credentials should login user'")]
        public async Task Login_WithValidCredentials_ShouldLoginUser()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {Headless = false, SlowMo = 50,});
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();
            await page.GotoAsync(HttpsWwwDemoblazeCom);
            await page.ClickAsync(AHasTextLogIn);
            await Helpers.Login("tyschenk@20@gmail.com", "12345678", page);
            await page.ClickAsync("id=nameofuser");
            var welcomeText = await page.TextContentAsync("id=nameofuser");
            var visibilityOfLogOut = await page.IsVisibleAsync("id=logout2");
            var visibilityOfLogIn = await page.IsVisibleAsync("id=login");
            Assert.AreEqual("Welcome tyschenk@20@gmail.com", welcomeText);
            Assert.True(visibilityOfLogOut);
            Assert.False(visibilityOfLogIn);
        }
        
    }
}