using System;
using System.Collections.Generic;
using System.Text;

namespace Kdoctl.Schema.CliServices
{
    public class ConsoleLogger
    {
        public void StatusBegin(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(message);
        }

        public void StatusEndSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }
        public void StatusEndFailed(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }

        public void Message(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(message);
        }

        public void NewLineMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
        }

        public void SilentError(string message)
        {
            // Doing nothing for now
        }
    }
}
