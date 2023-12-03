using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using SeleniumTest1.Extensions;

namespace SeleniumTest1
{
    internal class WinFormsGitHubFind
    {
        private readonly ChromeDriver _driver;
        private readonly INavigation _nav;
        public string SearchUrl { get; set; }
        internal WinFormsGitHubFind(string searchUrl, bool useUserData = true) 
        {
            SearchUrl = searchUrl;
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

        ~WinFormsGitHubFind()
        {
            try
            {
                _driver?.CloseDevToolsSession();
                _driver?.Dispose();
                _driver?.Close();
            }
            catch {; }
        }


        public string FetchAllOccurances()
        {
            StringBuilder myBuilder = new();
            _nav.GoToUrl(SearchUrl);

            var allMarksElement = _driver.FindElements(By.TagName("mark"))
                .Where(el => el.Text == "AddRange")
                .Select(el => el.GetParent()?.GetAttribute("innerHTML")?.Trim()).ToArray();
            var myStr = allMarksElement[1]
                ?.Split(
                    new string[] {
                        "</span>", "<mark>", "</mark>", "/span", "<span", "\">"
                    }, StringSplitOptions.RemoveEmptyEntries)
                .Select(text => text.Trim())
                .Where(text => !string.IsNullOrWhiteSpace(text) &&
                    !text.StartsWith("/") &&
                    !text.StartsWith("span") && 
                    !text.StartsWith("class=") &&
                    text.Contains("public") &&
                    !text.Contains("params"))
                .ToArray().JoinProgrammicParts().Replace("public void", "");
            return myBuilder.ToString();
        }
    }
}
