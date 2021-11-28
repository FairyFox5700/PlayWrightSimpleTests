using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace PlayWrightNUnit
{
    public class BrowserFixture
    {
    public async Task WithPageAsync(Func<IPage, Task> action, string browserType = "chromium", [CallerMemberName] string? testName = null)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await CreateBrowserAsync(playwright, browserType);
        var options = new BrowserNewContextOptions();
        var context = await browser.NewContextAsync(options);
        await context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true
        });
        var page = await context.NewPageAsync();
        page.Console += (_, e) => TestContext.Out.WriteLine(e.Text);
        page.PageError += (_, e) => TestContext.Out.WriteLine(e);
       
        try
        {
            await action(page);
        }
        catch (Exception)
        {
            await TryCaptureScreenshotAsync(page, testName!, browserType);
            await context.Tracing.StopAsync(new TracingStopOptions
            {
                Path= "trace.zip"
            });
            throw;
        }
        finally
        {
            await TryCaptureVideoAsync(page, testName!, browserType);
        }
    }
    
    protected virtual BrowserNewPageOptions CreatePageOptions()
    {
        var options = new BrowserNewPageOptions
        {
            IgnoreHTTPSErrors = true, 
            RecordVideoDir = "videos",
            RecordVideoSize = new RecordVideoSize() { Width = 1024, Height = 768 }
        };
        return options;
    }

    private static async Task<IBrowser> CreateBrowserAsync(IPlaywright playwright, string browserType)
    {
        var options = new BrowserTypeLaunchOptions();
        if (System.Diagnostics.Debugger.IsAttached)
        {
            options.Devtools = true;
            options.Headless = false;
            options.SlowMo = 250;
        }
        var split = browserType.Split(':');
        browserType = split[0];
        if (split.Length > 1)
        {
            options.Channel = split[1];
        }
        return await playwright[browserType].LaunchAsync(options);
    }

    private static string GenerateFileName(string testName, string browserType, string extension)
    {
        string os =
            OperatingSystem.IsLinux() ? "linux" :
            OperatingSystem.IsMacOS() ? "macos" :
            OperatingSystem.IsWindows() ? "windows" :
            "other";
        browserType = browserType.Replace(':', '_');
        string utcNow = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
        return $"{testName}_{browserType}_{os}_{utcNow}{extension}";
    }

    private async Task TryCaptureScreenshotAsync(
        IPage page,
        string testName,
        string browserType)
    {
        try
        {
            string fileName = GenerateFileName(testName, browserType, ".png");
            string path = Path.Combine("screenshots", fileName);

            await page.ScreenshotAsync(new() { Path = path });

            TestContext.Out.WriteLine($"Screenshot saved to {path}.");
        }
        catch (Exception ex)
        {
            TestContext.Out.WriteLine("Failed to capture screenshot: " + ex);
        }
    }

    private async Task TryCaptureVideoAsync(IPage page, string testName, string browserType)
    {
        try
        {
            await page.CloseAsync();

            string videoSource = await page.Video!.PathAsync();

            string directory = Path.GetDirectoryName(videoSource);
            string extension = Path.GetExtension(videoSource);

            string fileName = GenerateFileName(testName, browserType, extension!);

            string videoDestination = Path.Combine(directory!, fileName);

            File.Move(videoSource, videoDestination);

            TestContext.Out.WriteLine($"Video saved to {videoDestination}.");
        }
        catch (Exception ex)
        {
            TestContext.Out.WriteLine("Failed to capture video: " + ex);
        }
    }
}
}