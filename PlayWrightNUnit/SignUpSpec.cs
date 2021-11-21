using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using PlayWrightNUnit.Attributes;

namespace PlayWrightNUnit
{
    [TestFixture]
    public class SignUpSpec
    {
        private const string HttpsWwwDemoblazeCom = @"https://www.demoblaze.com/";
        private const string AHasTextSignUp = "a:has-text('Sign Up')";
        private const string IdSignUsername = "id=sign-username";
        private const string IdSignPassword = "id=sign-password";
        private const string ButtonHasTextSignUp = "button:has-text('Sign up')";

        [Test,
         PlaywrightTest(
             nameof(SignUpSpec),
             "Verify types sign up with existing credentials should display dialog'")]
            public async Task SignUp_WithExistingCredentials_ShouldAlertMessage()
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
                    Assert.AreEqual("This user already exist.", e.Message);
                    await e.AcceptAsync();
                };
                await page.GotoAsync(HttpsWwwDemoblazeCom);
                await page.ClickAsync(AHasTextSignUp);

                await SignUp("tyschenk@20@gmail.com", "12345678", page);
            }
            
            [Test,
             PlaywrightTest(
                 nameof(SignUpSpec),
                 "Verify unexisting credentials should sign up successfully")]
            public async Task SignUp_WithUnExistingUser_ShouldSignUpSuccessfully()
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
                    Assert.AreEqual("Sign up successful.", e.Message);
                    await e.AcceptAsync();
                };
                await page.GotoAsync(HttpsWwwDemoblazeCom);
                await page.ClickAsync(AHasTextSignUp);
                await SignUp($"tyschenk{Guid.NewGuid()}@gmail.com", "12345678", page);
            }

            private async Task SignUp(string userName, string password, IPage page)
            {
                if (userName == null) throw new ArgumentNullException(nameof(userName));
                if (password == null) throw new ArgumentNullException(nameof(password));
                await page.ClickAsync(IdSignUsername);
                await page.TypeAsync(IdSignUsername, userName);
                await page.ClickAsync(IdSignPassword);
                await page.TypeAsync(IdSignPassword, password);
                await page.ClickAsync(ButtonHasTextSignUp);
            }
    }
}