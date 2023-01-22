using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Threading;

namespace SeleniumTest1
{
    public class Exam1
    {
        public static string GetExamQuestionAndAnswers(string examUrl)
        {
            var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArgument("--headless");

            var driver = new ChromeDriver(chromeOptions);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(6000);

            var nav = driver.Navigate();

            nav.GoToUrl(examUrl);

            //var goToExamBtn = driver.FindElement(By.ClassName("btn btn-primary btn-lg"));
            var goToExamBtn = driver.FindElement(By.CssSelector(".btn.btn-primary.btn-lg"));

            if (goToExamBtn == null || goToExamBtn.GetAttribute("disabled") == "true")
            {
                throw new InvalidOperationException("Go to exam btn does not exist or is disabled");
            }

            goToExamBtn.Click();

            var revealSolutionButtons = driver.FindElements(By.CssSelector(".btn.btn-primary.reveal-solution.d-print-none"));
            foreach (var currentBtn in revealSolutionButtons)
            {
                currentBtn.Click();
                Thread.Sleep(300);
            }

            var answersCount = revealSolutionButtons.Count;
            Console.WriteLine($"Answers count is {answersCount}");

            var totalStr = new StringBuilder();


            var questionIndex = 1;
            var currentPageNum = 0;
            while (true)
            {
                var currentPageQuestionIndex = 0;
                currentPageNum++;
                bool needsAnswer = false;
                var cardTexts = driver.FindElements(By.ClassName("card-text"));
                var allQuestionOptions = driver.FindElements(By.ClassName("multi-choice-item"));
                List<List<string>> questionsOptions = new();

                foreach (var currentOption in allQuestionOptions)
                {
                    //var theLetter = currentOption.FindElement(
                    //    By.ClassName("multi-choice-letter"))
                    //    .GetAttribute("data-choice-letter").Trim();
                    var currentTxt = currentOption.GetAttribute("innerText").Trim();

                    if (currentTxt.StartsWith("A"))
                        questionsOptions.Add(new() { currentTxt });
                    else
                        questionsOptions[^1].Add(currentTxt);
                }

                for (int i = 0; i < cardTexts.Count; i++)
                {
                    var currentCard = cardTexts[i];
                    var currentNodeName = currentCard.GetAttribute("nodeName");
                    if (string.Compare(currentNodeName, "p", true) == -1)
                        continue;

                    if (needsAnswer)
                    {
                        var answer = PurifyAnswer($"{currentCard.GetAttribute("innerText")}");
                        totalStr.AppendLine($"Correct Answer: {answer}");
                    }
                    else
                    {
                        totalStr.AppendLine($"{questionIndex}: " +
                            $"{currentCard.GetAttribute("innerText").Trim()}");

                        foreach (var currentOption in questionsOptions[currentPageQuestionIndex])
                            totalStr.AppendLine(currentOption);

                        questionIndex++;
                        currentPageQuestionIndex++;
                    }

                    needsAnswer = !needsAnswer;
                }

                totalStr.AppendLine($"Page {currentPageNum}");
                totalStr.AppendLine("=====================================");

                IWebElement? nextButton = null;
                try
                {
                    // try to go to the next page
                    nextButton = driver.FindElement(By.CssSelector(".btn.btn-success.pull-right"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Some unknown error had happened: ");
                    Console.WriteLine($"{ex}");
                    Console.WriteLine("Do whatever stuff you want, I will wait.");
                    Console.WriteLine("Send ok if you have fixed the issue, otherwise I will exit.");
                    if (Console.ReadLine() != "ok")
                    {
                        break;
                    }
                    nextButton = driver.FindElement(By.CssSelector(".btn.btn-success.pull-right"));
                }

                if (nextButton == null)
                {
                    Console.WriteLine("Reached end of the pages!");
                    break;
                }
                else if (nextButton.GetAttribute("disabled") == "true")
                {
                    Console.WriteLine("Next button exists but is disabled!");
                    break;
                }

                Thread.Sleep(500);
                nextButton.Click();

                try
                {
                    var robotCandidates = driver.FindElements(By.CssSelector(".col-12.text-center"));
                    if (robotCandidates == null || robotCandidates.Count == 0)
                        // all is good
                        continue;

                    if (robotCandidates.Any(
                        candidate =>
                        string.Compare(candidate.GetAttribute("innerText"),
                            "Are you a robot?", true) == 0))
                    {
                        Console.WriteLine("There is a captcha on the screen.");
                        Console.WriteLine("Solve it and send an input here for me.");
                        Console.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return totalStr.ToString(); ;
        }
    
        private static string PurifyAnswer(string value)
        {
            var result = "";
            foreach (var currentChar in value)
            {
                if (char.IsLetter(currentChar))
                    result += currentChar;
            }

            result = result.ToLower().Replace("correct", "").Replace("answer", "");
            return result.ToUpper();
        }
    }
}
