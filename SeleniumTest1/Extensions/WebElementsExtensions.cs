using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace SeleniumTest1.Extensions
{
    public static class WebElementsExtensions
    {
        public static IWebElement? GetParent(this IWebElement element)
        {
            try
            {
                return element.FindElement(By.XPath("./.."));
            }
            catch
            {
                return null;
            }
        }
        public static ReadOnlyCollection<IWebElement>? FindElementsSafe(
            this ISearchContext element, By by)
        {
            try
            {
                return element?.FindElements(by);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no element"))
                    return null;
                throw;
            }
        }
        public static float GetNumericText(this IWebElement element)
        {
            try
            {
                return Convert.ToSingle(element.Text.GetNumericChars("0"));
            }
            catch { return 0; }
        }
        public static IWebElement? FindElementSafe(
            this ISearchContext element, By by)
        {
            try
            {
                return element?.FindElement(by);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no element"))
                    return null;
                throw;
            }
        }
        public static bool IsChapterText(this string text) =>
            text.Contains("chapter", StringComparison.OrdinalIgnoreCase) ||
            text.Contains("episode", StringComparison.OrdinalIgnoreCase);
    }
}
