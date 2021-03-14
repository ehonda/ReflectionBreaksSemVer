using System;
using System.Linq;
using System.Reflection;

namespace ReflectionBreaksSemVer
{
    class Program
    {
        static void PrintLibraryInfo()
        {
            var library = Assembly.LoadFrom("SomeLib.dll");

            var types = library.ExportedTypes;

            foreach (var type in types)
            {
                Console.WriteLine($"{type.FullName}");
                foreach (var method in type.GetMethods())
                {
                    Console.WriteLine($"{method.Name}");
                }
            }
        }

        static bool BrokenByAdditionalType()
            => Assembly
                .LoadFrom("SomeLib.dll")
                .ExportedTypes
                .Count() == 1;

        static bool BrokenByAdditionalMethod()
            => Assembly
                .LoadFrom("SomeLib.dll")
                .ExportedTypes
                .Single(type => type.Name == "ClassA")
                .GetMethods()
                .Length == 5;

        static void Main(string[] args)
        {
            PrintLibraryInfo();
            Console.WriteLine();
            Console.WriteLine($"{BrokenByAdditionalType()}");
            Console.WriteLine($"{BrokenByAdditionalMethod()}");
        }
    }
}
