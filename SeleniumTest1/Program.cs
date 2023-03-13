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
            if (args.Length < 1) 
            {
                PrintHelp();
                return;
            }

            switch (args[0])
            {
                case "0":
                    var output = Exam1.GetExamQuestionAndAnswers("https://www.examtopics.com/exams/vmware/5v0-61-22/");
                    File.WriteAllText("output.txt", output);
                    break;
                case "1":
                    var ytOuput = WinuallVids.GetAllYtLinks("https://anilkhannasacademyoflaw.winuall.com/", args[0], args[1]);
                    File.WriteAllLines("output-links.txt", ytOuput);
                    break;
                default:
                    Console.WriteLine($"Unknown test kind: {args[0]}");
                    PrintHelp();
                    return;
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Welcome to SeleniumTest1!");
            Console.WriteLine("This is a collection of simple selenium tests!");
            Console.WriteLine("As the first argument, you have to enter which test you want to run.");
            Console.WriteLine("--------------");
            Console.WriteLine("Here are your choices");
            Console.WriteLine("0: examtopics");
            Console.WriteLine("1: winuall");
        }
    }
}