using System;

namespace DotNetCoreAdDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ADHelper helper = new ADHelper();
            helper.Sync();

            Console.WriteLine("Success");
        }
    }
}
