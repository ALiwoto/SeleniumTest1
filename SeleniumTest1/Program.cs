using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;

namespace SeleniumTest1
{
    public class Program
    {
        static void Main(string[] args)
        {
            var output = Exam1.GetExamQuestionAndAnswers("https://www.examtopics.com/exams/vmware/5v0-61-22/");
            File.WriteAllText("output.txt", output);
        }
    }
}