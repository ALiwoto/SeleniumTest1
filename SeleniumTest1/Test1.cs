using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace SeleniumTest1
{
    public class Test1
    {
        public static void DoOperation()
        {
            var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArgument("--headless");

            var driver = new ChromeDriver(chromeOptions);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(5000);

            var nav = driver.Navigate();

            nav.GoToUrl("https://www.selenium.dev/selenium/web/web-form.html");

            //var textBox = driver.FindElement(By.Name("my-text"));
            var textBox = driver.FindElement(By.Id("my-text-id"));
            var submitButton = driver.FindElement(By.TagName("button"));

            var disabledInput = driver.FindElement(By.Name("my-disabled"));
            if (disabledInput.GetAttribute("disabled") != "true")
            {
                disabledInput.SendKeys("test for disabled");
            }

            var myClass = disabledInput.GetAttribute("class");

            var myChecks = driver.FindElements(By.Name("my-check"));
            foreach (var myCheck in myChecks)
            {
                if (myCheck.GetAttribute("checked") != "true")
                {
                    myCheck.Click();
                }
            }

            var myColor = driver.FindElement(By.Name("my-colors"));

            var theColorValue = myColor.GetAttribute("value");

            var result = driver.ExecuteScript($"document.getElementsByName('{myColor.GetAttribute("name")}')[0].value='#FFC0CB'");

            textBox.SendKeys("Selenium");
            //submitButton.Click();

            Console.WriteLine("Hello, World!");

            driver.Quit();
        }
    }
}
