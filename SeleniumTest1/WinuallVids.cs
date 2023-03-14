using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Interactions;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;

namespace SeleniumTest1
{
    internal class WinuallVids
    {
        private ChromeDriver _driver;
        private string _baseUrl;
        private INavigation _nav;
        private List<VideoContainer> _videoContainers;

        private WinuallVids(string baseUrl)
        {
            _baseUrl = baseUrl;
            var chromeOptions = new ChromeOptions();
            var chromeDataPath = AppContext.BaseDirectory + "\\sel-user-data";

            if (Environment.GetEnvironmentVariable("automated_test") == "true")
            {
                chromeOptions.AddArgument("--headless");
                chromeOptions.AddArgument("--no-sandbox");
                chromeOptions.AddArguments("--disable-dev-shm-usage");

            }
            chromeOptions.AddArgument($"--user-data-dir={chromeDataPath}");


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
            _videoContainers = new();
        }
        ~WinuallVids()
        {
            try
            {
                _driver?.CloseDevToolsSession();
                _driver?.Dispose();
                _driver?.Close();
            }
            catch (Exception) { ; }
        }
        private void FetchYtLinks(string username, string password)
        {
            _nav.GoToUrl(_baseUrl);
            IWebElement? loginButton = null;
            try
            {
                loginButton = _driver.FindElement(By.XPath("//*[@id=\"storeDetailsHeader\"]/div[2]/button"));
            }
            catch {; }

            if (loginButton != null && loginButton.Text == "Log In" && loginButton.Enabled)
            {

                loginButton.Click();
                DoLogin(username, password);
            }

            Thread.Sleep(5000);

            var newContentElement = _driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div/section/section/main/div[2]/div/div/div[2]/div/div/div/div/div[3]/div[2]/div/div/div[1]/div"));
            if (newContentElement == null)
            {
                throw new InvalidOperationException(
                    "new content button not found");
            }
            newContentElement.Click();

            Thread.Sleep(5000);

            var noOfvideoColumn = _driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div/section/section/main/div/div/div[2]/div/div/div/div[2]/div/div/table/tbody/tr[2]/td[3]/div"));
            noOfvideoColumn.Click();

            var currentFolders = GetCurrentFolders();
            if (currentFolders == null || currentFolders.Count == 0)
            {
                return;
            }

            var currentUrl = _driver.Url;
            for (int i = 0; i < currentFolders.Count; i++)
            {
                IncreaseHeight();
                Thread.Sleep(2000);
                Console.WriteLine($"Detected total tr of {currentFolders.Count}");

                var toBeClicked = _driver?.FindElements(By.TagName("tbody"))
                    ?.Where(el => el.GetAttribute("data-test-id") == "virtuoso-item-list")
                    ?.FirstOrDefault()
                    ?.FindElements(By.TagName("tr"))[i]
                    ?.FindElement(By.TagName("td"));
                if (toBeClicked == null)
                    continue;

                var folderName = GetFolderName(toBeClicked!);
                toBeClicked.Click();


                CrawlInFolders($"/{folderName}/");
                _nav.GoToUrl(currentUrl);
            }

        }
        private List<string> FormatAsLinesOfFile()
        {
            var allLines = new List<string>();
            foreach (var currentVid in _videoContainers)
            {
                allLines.Add(currentVid.Path);
                allLines.Add(currentVid.Path);
                allLines.Add("======================");
            }

            Console.WriteLine($"Total length of video links: {_videoContainers.Count}");
            return allLines;
        }
        public static List<string> GetAllYtLinks(string baseUrl, string username, string password)
        {
            var winualObj = new WinuallVids(baseUrl);

            winualObj.FetchYtLinks(username, password);
            return winualObj.FormatAsLinesOfFile();
        }
        private void CrawlInFolders(string currentPath)
        {
            Thread.Sleep(2000);
            IncreaseHeight();
            var currentFolders = GetCurrentFolders();
            if (currentFolders == null || currentFolders.Count == 0)
            {
                // it means the folder is empty.
                ExtractVideoLinkFromPage(currentPath);
                return;
            }

            Console.WriteLine($"Detected total folders of {currentFolders.Count}");
            var currentUrl = _driver?.Url;
            for (int i = 0; i < currentFolders.Count; i++)
            {
                IncreaseHeight();
                

                //var toBeClicked = _driver?.FindElement(By.XPath($"//*[@id=\"root\"]/div[1]/div/div/section/section/main/div/div/div[2]/div[3]/div/div/div/div/div/table/tbody/tr[1]/td[{i + 1}]"));
                var toBeClicked = _driver?.FindElements(By.TagName("tbody"))
                    ?.Where(el => el.GetAttribute("data-test-id") == "virtuoso-item-list")
                    ?.FirstOrDefault()
                    ?.FindElements(By.TagName("tr"))[i]
                    ?.FindElement(By.TagName("td"));
                var folderName = GetFolderName(toBeClicked!);
                toBeClicked?.Click();

                CrawlInFolders(currentPath + $"{folderName}/");

                _driver?.Navigate().GoToUrl(currentUrl);
            }
        }

        private void ExtractVideoLinkFromPage(string path)
        {
            path = path.TrimEnd('/') + ".mkv";
            var ytIFrame = _driver?.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div/section/section/main/div/div/div[2]/div/div/div[2]/div/div[1]/iframe"));
            if (ytIFrame == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING: could not find any iFrame.");
                Console.ResetColor();
                return;
            }

            var srcAttr = ytIFrame.GetAttribute("src");
            var vidLink = DelEmbed(new Uri(srcAttr).LocalPath);
            if (string.IsNullOrEmpty(vidLink))
                throw new InvalidOperationException(
                    $"Invalid video link extracted from srcAttr: {vidLink}");

            Console.WriteLine($"Adding path of {path} to collection.");
            _videoContainers.Add(new(path, vidLink, ""));
            Console.WriteLine(srcAttr);
        }

        private static string DelEmbed(string url) =>
            url.StartsWith("/embed/") ?  url["/embed/".Length..] : url;
        //private ReadOnlyCollection<IWebElement>? GetCurrentFolders(string className) =>
        //    _driver?.FindElements(By.CssSelector(className));
        private ReadOnlyCollection<IWebElement>? GetCurrentFolders()
        {
            Thread.Sleep(1000);
            IncreaseHeight();

            Thread.Sleep(4600);
            return _driver?.FindElements(By.TagName("tbody"))
                    ?.Where(el => el.GetAttribute("data-test-id") == "virtuoso-item-list")
                    ?.FirstOrDefault()
                    ?.FindElements(By.TagName("tr"));
        }

        private void IncreaseHeight()
        {
            _driver.ExecuteJavaScript("let myElements = document.getElementsByTagName(\"div\"); for (let i = 0; i < myElements.length; i++) { if (myElements[i].getAttribute(\"data-test-id\") == \"virtuoso-scroller\") { myElements[i].setAttribute(\"style\", \"height: 4000000000px; outline: none; overflow-y: auto; position: relative;\"); break; } };");
            Thread.Sleep(1600);
        }
        //_driver?.FindElements(By.TagName("tbody"))
        //            .Where(el => el.GetAttribute("data-test-id") == "virtuoso-item-list")
        //            .First()
        //            .FindElements(By.TagName("tr"))[i]
        //            .FindElement(By.TagName("td"));

        private static string GetFolderName(IWebElement columnElement) =>
            //columnElement.FindElement(By.CssSelector(".sc-iUVJNI.hfBOoT")).Text;
            columnElement.FindElements(By.TagName("div"))
            .Where(el => !string.IsNullOrEmpty(el.Text)).First().Text;

        private void DoLogin(string username, string password)
        {
            IWebElement? usernameTextBox = null;
            try
            {
                usernameTextBox = _driver?.FindElement(By.XPath("//*[@id=\"root\"]/div[1]/div/div/div/div[2]/div/form/div[1]/div/div[2]/div[1]/div/div/div/input"));
            }
            catch (Exception) { ; }
            if (usernameTextBox == null)
                return;

            Console.WriteLine("LOGIN: Starting login operation.");
            Console.WriteLine($"LOGIN: username len: {username.Length}");
            Console.WriteLine($"LOGIN: password len: {password.Length}");
            var passwordTextBox = _driver?.FindElement(By.XPath("/html/body/div/div[1]/div/div/div/div[2]/div/form/div[2]/div/div[2]/div[1]/div/div[1]/div/div/div/div/input"));
            var passwordSubmitButton = _driver?.FindElement(By.XPath("/html/body/div/div[1]/div/div/div/div[2]/div/form/div[3]/button"));

            usernameTextBox?.SendKeys(username);
            passwordTextBox?.SendKeys(password);
            passwordSubmitButton?.Click();
        }

        private class VideoContainer
        {
            internal string Path { get; set; }
            internal string Link { get; set; }

            internal string Title { get; set; }
            internal VideoContainer(string path, string link, string title)
            {
                if (!path.StartsWith("https://www.youtube.com/watch?v="))
                {
                    path = $"https://www.youtube.com/watch?v={path}";
                }
                Path = path;
                Link = link;
                Title = title;
            }
        }
    }
}
