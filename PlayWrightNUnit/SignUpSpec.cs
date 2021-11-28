using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using PlayWrightNUnit.Attributes;
using PlayWrightNUnit.PageObjects;

namespace PlayWrightNUnit
{
    [TestFixture]
    public class SignUpSpec
    {
        private IPlaywright playwright;
        private IBrowser browser;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {Headless = false, SlowMo = 50,});
        }
        [Test,
         PlaywrightTest(
             nameof(SignUpSpec),
             "Verify types sign up with existing credentials should display dialog'")]
            public async Task SignUp_WithExistingCredentials_ShouldAlertMessage()
            {
                var page = new SignUpPageObject(await browser.NewPageAsync());
                page.OnDialog += async (_, e) =>
                {
                    Assert.AreEqual(DialogType.Alert, e.Type);
                    Assert.AreEqual(string.Empty, e.DefaultValue);
                    Assert.AreEqual("This user already exist.", e.Message);
                    await e.AcceptAsync();
                };
                await page.GotoAsync();
                await page.ClickSignUpForm();
                await page.SignUp("tyschenk@20@gmail.com", "12345678");
            }
            
            [Test,
             PlaywrightTest(
                 nameof(SignUpSpec),
                 "Verify unexisting credentials should sign up successfully")]
            public async Task SignUp_WithUnExistingUser_ShouldSignUpSuccessfully()
            {
                var page = new SignUpPageObject(await browser.NewPageAsync());
                page.OnDialog += async (_, e) =>
                {
                    Assert.AreEqual(DialogType.Alert, e.Type);
                    Assert.AreEqual(string.Empty, e.DefaultValue);
                    Assert.AreEqual("Sign up successful.", e.Message);
                    await e.AcceptAsync();
                };
                await page.GotoAsync();
                await page.ClickSignUpForm();
                await page.SignUp($"tyschenk{Guid.NewGuid()}@gmail.com", "12345678");
            }
    }
}