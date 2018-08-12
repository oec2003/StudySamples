using System;

namespace NetCoreEnvironmentDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string name = Environment.GetEnvironmentVariable("name");
            string age = Environment.GetEnvironmentVariable("age");

            Console.WriteLine($"name:{name}");
            Console.WriteLine($"age:{age}");
            Console.ReadLine();
        }
    }
}
