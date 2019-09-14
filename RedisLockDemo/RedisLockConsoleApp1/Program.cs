using RedisLockLib;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedisLockConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            SeqNo.InitRedis();
            Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    Console.WriteLine($"Thread1:SeqNo:{SeqNo.GetSeqNoByRedisLock()}");
                }
            });

            Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    Console.WriteLine($"Thread2:SeqNo:{SeqNo.GetSeqNoByRedisLock()}");
                }
            });
            Console.ReadLine();
        }
    }
}
