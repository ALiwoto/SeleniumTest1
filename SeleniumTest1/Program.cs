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
                    var username = Environment.GetEnvironmentVariable("TARGET_USERNAME") ?? args[1];
                    var password = Environment.GetEnvironmentVariable("TARGET_PASSWORD") ?? args[2];
                    var ytOuput = WinuallVids.GetAllYtLinks("https://anilkhannasacademyoflaw.winuall.com/", username, password);
                    File.WriteAllLines("output-links.txt", ytOuput);
                    break;
                case "2":
                    var gh = new WinFormsGitHubFind("https://github.com/search?q=repo%3Adotnet%2Fwinforms%20AddRange&type=code");
                    File.WriteAllText("output-gh", gh.FetchAllOccurances());
                    break;
                case "3":
                    var game2048 = new Selenium2048("https://sui8192.ethoswallet.xyz/");
                    File.WriteAllLines("game2048Moves", game2048.DoPlay2048()!);
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
            Console.WriteLine("2: 2048 game");
        }
    }
}