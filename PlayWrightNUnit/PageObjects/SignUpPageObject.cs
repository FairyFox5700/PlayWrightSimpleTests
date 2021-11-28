using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace PlayWrightNUnit.PageObjects
{
    public class SignUpPageObject
    {
        private readonly IPage _page;
        private const string HttpsWwwDemoblazeCom = @"https://www.demoblaze.com/";
        private const string AHasTextSignUp = "a:has-text('Sign Up')";
        private const string IdSignUsername = "id=sign-username";
        private const string IdSignPassword = "id=sign-password";
        private const string ButtonHasTextSignUp = "button:has-text('Sign up')";

        public SignUpPageObject(IPage page)
        {
            _page = page;
            _page.Dialog+=OnDialog; 
        }

        public event  EventHandler<IDialog> OnDialog;
        
        public async Task GotoAsync()
        {
            await _page.GotoAsync(HttpsWwwDemoblazeCom);
        }

        public async Task ClickSignUpForm()
        {
            await _page.ClickAsync(AHasTextSignUp); 
        }
        
        public async Task SignUp(string userName, string password)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));
            await _page.ClickAsync(IdSignUsername);
            await _page.TypeAsync(IdSignUsername, userName);
            await _page.ClickAsync(IdSignPassword);
            await _page.TypeAsync(IdSignPassword, password);
            await _page.ClickAsync(ButtonHasTextSignUp);
        }
    }
    
}