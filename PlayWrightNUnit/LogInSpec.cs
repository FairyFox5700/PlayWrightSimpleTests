using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlayWrightNUnit.Attributes;
using PlayWrightNUnit.PageObjects;

namespace PlayWrightNUnit
{
    [TestFixture]
    public class LoginSpec
    {
        private IBrowserContext _context;
        private LoginPageObject page;
        private IBrowser _browser;


        [SetUp]
        public async Task SetUp()
        {

            var playwright = await Playwright.CreateAsync();
            var chromium = playwright.Chromium;
            _browser = await chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
#if DEBUG
                Headless = false,
                Devtools = true,
                SlowMo = 1000
#endif
            });

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = "videos/",
            });
            
            page = new LoginPageObject(await _browser.NewPageAsync());
       
        }
        
        [TearDown]
        public async Task Cleanup()
        {
            await _browser.CloseAsync();
            await _context.CloseAsync();
        }
        [Test,
         PlaywrightTest(
             nameof(LoginSpec),
             "When empty login and password should respond with alert that contains text " +
             "'Please fill out Username and Password.'")]
        public async Task Login_WithEmptyUserNameAndPassword_ShouldAlertMessage()
        {
           
            page.Page.Dialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Type);
                Assert.AreEqual(string.Empty, e.DefaultValue);
                Assert.AreEqual("Please fill out Username and Password.", e.Message);
                await e.AcceptAsync();
            };
            await page.GotoAsync();
            await page.ClickLoginForm();
            await page.Login("","");
        }
        
        [Test,
         PlaywrightTest(
             nameof(LoginSpec),
             "When empty login and password should respond with alert that contains text " +
             "'Please fill out Username.'")]
        public async Task Login_WithEmptyUserNameAndPassword_ShouldAlertInCorrectMessage()
        {
            var browser = new BrowserFixture();
            await browser.WithPageAsync(async pagee =>
            {
    
               var page = new LoginPageObject(pagee);
          
            string message = "";
            page.Page.Dialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Type);
                Assert.AreEqual(string.Empty, e.DefaultValue);
                message = e.Message;
                await e.AcceptAsync();
            };
            await page.GotoAsync();
            await page.ClickLoginForm();
            await page.Login("","");
            Assert.AreEqual("Please fill out Username.",message );
            },"chromium");
        }

        [Test,
         PlaywrightTest(
             nameof(LoginSpec),
             "When types unexisting email 'User does not exist.'")]
        public async Task Login_WithUnExistingEmail_ShouldAlertMessage()
        {
            var page = new LoginPageObject(await _browser.NewPageAsync());
            page.Page.Dialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Type);
                Assert.AreEqual(string.Empty, e.DefaultValue);
                Assert.AreEqual("User does not exist.", e.Message);
                await e.AcceptAsync();
            };
            await page.GotoAsync();
            await page.ClickLoginForm();
            await page.Login("t@ggg.com", "1234567");
        }


        [Test,
         PlaywrightTest(nameof(LoginSpec), "When types invalid password 'User does not exist.'")]
        public async Task Login_WithUnInvalidPassword_ShouldAlertMessage()
        {   
            var page = new LoginPageObject(await _browser.NewPageAsync());
            page.Page.Dialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Type);
                Assert.AreEqual(string.Empty, e.DefaultValue);
                Assert.AreEqual("Wrong password.", e.Message);
                await e.AcceptAsync();
            };
            await page.GotoAsync();
            await page.ClickLoginForm();
            await page.Login("t@gmail.com", "123456");
        }

        [Test,
         PlaywrightTest(nameof(LoginSpec), "When types valid credentials should login user'")]
        public async Task Login_WithValidCredentials_ShouldLoginUser()
        {
            var page = new LoginPageObject(await _browser.NewPageAsync());
            await page.GotoAsync();
            await page.ClickLoginForm();
            await page.Login("tyschenk@20@gmail.com", "12345678");
            
            var welcomeText = await page.WelcomeText();
           
            var visibilityOfLogOut = await page.GetVisibility("id=logout2");
            var visibilityOfLogIn = await page.GetVisibility("id=login");
            Assert.AreEqual("Welcome tyschenk@20@gmail.com", welcomeText);
            Assert.True(visibilityOfLogOut);
            Assert.False(visibilityOfLogIn);
        }
        
    }
}