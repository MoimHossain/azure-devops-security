

using System;
using System.Collections.Generic;
using System.Text;

namespace Kdoctl.Schema.CliServices
{
    public class ConsoleLogger
    {
        public static void StatusBegin(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(message);
        }

        public static void StatusEndSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }
        public static void StatusEndFailed(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }

        public static void Message(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(message);
        }

        public static void NewLineMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
        }

        public static void SilentError(string message)
        {
            Console.WriteLine(message);
        }
    }
}
