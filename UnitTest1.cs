using System;
using Xunit;

namespace Playwrightdemo
{
    
    public static class WebConstants
    {
        public static string Username { get; } = "NigmaOmega";
        public static string Password { get; } = "Password@123";
        public static string BaseUrl { get; } = "https://www.demoblaze.com";
        public static string HomePage { get; } = BaseUrl + "/index.html";

    }
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
        }
    }
}