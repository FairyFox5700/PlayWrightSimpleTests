using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlayWrightNUnit.PageObjects
{
    public class LoginPageObject
    {
        #region constant
        private const string HttpsWwwDemoblazeCom = @"https://www.demoblaze.com/";
        private const string IdLoginusername = "id=loginusername";
        private const string IdLoginpassword = "id=loginpassword";
        private const string AHasTextLogIn = "a:has-text('Log In')";
        private const string IdNameofuser = "id=nameofuser";
        private readonly IPage _page;
        #endregion
        public LoginPageObject(IPage page)
        {
            _page = page;
        }
        public IPage Page => _page;

        public async Task GotoAsync()
        {
            await _page.GotoAsync(HttpsWwwDemoblazeCom);
        }

        public async Task ClickLoginForm()
        {
            await _page.ClickAsync(AHasTextLogIn); 
        }
        public async Task Login(string userName, string password)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));
          
            await _page.ClickAsync(IdLoginusername);
            await _page.TypeAsync(IdLoginusername, userName);
            await _page.ClickAsync(IdLoginpassword);
            await _page.TypeAsync(IdLoginpassword, password);
            await _page.ClickAsync("button:has-text('Log in')");
        }

   
        public async Task<string> WelcomeText()
        {
            await _page.ClickAsync(IdNameofuser);
            return await _page.TextContentAsync(IdNameofuser);
        }

        public async Task<bool?> GetVisibility(string field)
        {
            return await _page.IsVisibleAsync(field);
        }
    }
}