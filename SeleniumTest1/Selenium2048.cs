using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;

namespace SeleniumTest1
{
    internal class Selenium2048
    {
        private readonly ChromeDriver _driver;
        private readonly INavigation _nav;
        public string PageUrl { get; set; }
        internal Selenium2048(string pageUrl, bool useUserData = true)
        {
            PageUrl = pageUrl;
            var chromeOptions = new ChromeOptions();


            //if (Environment.GetEnvironmentVariable("automated_test") == "true")
            {
                //chromeOptions.AddArgument("--headless=new");
                //chromeOptions.AddArgument("--no-sandbox");
                //chromeOptions.AddArgument("--disable-web-security");
                //chromeOptions.AddArguments("--disable-dev-shm-usage");

            }
            if (useUserData)
            {
                var chromeDataPath = AppContext.BaseDirectory + "\\sel-user-data";
                chromeOptions.AddArgument($"--user-data-dir={chromeDataPath}");
            }


            try
            {
                _driver = new ChromeDriver(
                    ChromeDriverService.CreateDefaultService(),
                    chromeOptions,
                    TimeSpan.FromSeconds(360.0));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"First try failed: {ex.Message}");

                var chromePath = new DriverManager().SetUpDriver(new ChromeConfig());
                _driver = new ChromeDriver(
                    ChromeDriverService.CreateDefaultService(chromePath),
                    chromeOptions,
                    TimeSpan.FromSeconds(360.0));
            }


            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(6000);

            _nav = _driver.Navigate();
        }

        ~Selenium2048()
        {
            try
            {
                _driver?.CloseDevToolsSession();
                _driver?.Dispose();
                _driver?.Close();
            }
            catch {; }
        }


        public string[]? DoPlay2048()
        {
            Console.WriteLine("ok");
            return null;
        }
    }
}
