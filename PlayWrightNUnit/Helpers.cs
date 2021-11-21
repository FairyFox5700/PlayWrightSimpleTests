using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlayWrightNUnit
{
    public static class Helpers
    {
        private const string IdLoginusername = "id=loginusername";
        private const string IdLoginpassword = "id=loginpassword";

        public static async Task Login(string userName, string password, IPage page)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));
            await page.ClickAsync(IdLoginusername);
            await page.TypeAsync(IdLoginusername, userName);
            await page.ClickAsync(IdLoginpassword);
            await page.TypeAsync(IdLoginpassword, password);
            await page.ClickAsync("button:has-text('Log in')");
        }

        public static async Task ClearLogin(IPage page)
        {
            await page.ClickAsync(IdLoginusername);
            await page.PressAsync(IdLoginusername, "Ctrl+A Delete");

            await page.ClickAsync(IdLoginpassword);
            await page.PressAsync(IdLoginpassword, "Ctrl+A Delete");
        }
    }
}