using SomeLib;
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
                Console.WriteLine();
                foreach (var method in type.GetMethods())
                {
                    Console.WriteLine($"{method.Name}");
                }
                Console.WriteLine();
                Console.WriteLine("-----------------------");
            }
        }

        // Broken by an additional type exported from SomeLib
        static bool Foo()
            => Assembly
                .LoadFrom("SomeLib.dll")
                .ExportedTypes
                .Count() == 1;

        // Broken by an additional method provided by ClassA
        static bool Bar()
            => Assembly
                .LoadFrom("SomeLib.dll")
                .ExportedTypes
                .Single(type => type.Name == nameof(ClassA))
                .GetMethods()
                .Length == 5;

        static void Main(string[] args)
        {
            PrintLibraryInfo();
            Console.WriteLine();
            Console.WriteLine($"Foo: {Foo()}");
            Console.WriteLine($"Bar: {Bar()}");
        }
    }
}
