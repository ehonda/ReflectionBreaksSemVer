using System;
using System.Reflection;

namespace ReflectionBreaksSemVer
{
    class Program
    {
        static void Main(string[] args)
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
    }
}
