using Cielo.Manifests.Common;
using Microsoft.VisualStudio.Services.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cielo.CliSupports
{
    public class ConsoleReport
    {
        public static void Begin()
        {
            Console.Clear();
            Console.ResetColor();
        }

        public static void End()
        {
            Console.ResetColor();
        }

        public static void BeginResource(string? name, ManifestKind kind)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" □ {kind} : ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{name}");
        }

        public static void ResourceState(bool exists)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("   State : ");
            if (exists)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" ✔ Already exists.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($" ✖ Doesn't exist, will be provisioned. ");
            }     
        }

        public static void ReportBeforeStateProperty((string, object, bool) property)
        {
            Console.Write("\t");
            var name = property.Item1;
            var value = property.Item2;
            var changed = property.Item3;

            if (changed)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($" ≈ ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($" ☛ ");
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{name} : ");
            Console.ForegroundColor = changed ? ConsoleColor.DarkMagenta: ConsoleColor.DarkYellow;
            Console.WriteLine($"{value}");
        }

        public static void ReportAfterStateProperty((string, object, bool) property)
        {
            Console.Write("\t");
            var name = property.Item1;
            var value = property.Item2;
            var changed = property.Item3;

            if (changed)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($" ≈ ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($" + ");
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{name} : ");
            Console.ForegroundColor = changed ? ConsoleColor.DarkGreen : ConsoleColor.Green;
            Console.WriteLine($"{value}");
        }

        public static void ReportError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" ♨ ERROR: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{error}");
            Console.ResetColor();
        }

        public static void ChangeBegin(string? name, ManifestKind kind)
        {   
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" ☀ Mutations ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{name}");
        }
    }
}
