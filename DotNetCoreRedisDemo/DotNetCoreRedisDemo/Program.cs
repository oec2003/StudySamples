using System;
using System.Collections.Generic;
using CSRedis;

namespace DotNetCoreRedisDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string redisServerIP = "172.16.0.13";
            string redisServerPort = "6380";
            string redisPassword = "";
            bool isSentinelMode = false;

            string connectionString = GetRedisConnectionString(redisServerIP, redisServerPort, redisPassword,0, "", isSentinelMode);
            //哨兵模式
            if (isSentinelMode)
            {
                redisServerIP = "172.16.0.13,172.16.0.13"; //哨兵IP列表
                redisServerPort = "26379,26380";
                List<string> connectionList = GetRedisConnectionList(redisServerIP, redisServerPort);
                CSRedisClient csredis = new CSRedisClient(connectionString, connectionList.ToArray());
                RedisHelper.Initialization(csredis);//初始化
            }
            else
            {
                //普通模式，连接主库
                CSRedisClient csredis = new CSRedisClient(connectionString);
                RedisHelper.Initialization(csredis);//初始化
            }
            Console.WriteLine("Hello World!");
        }

        private static List<string> GetRedisConnectionList(string ips, string ports)
        {
            List<string> connectionList = new List<string>();

            for (int i = 0; i < ips.Split(',').Length; i++)
            {
                string ip = ips.Split(',')[i];
                string port = ports.Split(',')[i];

                connectionList.Add($"{ip}:{port}");
            }

            return connectionList;
        }

        private static string GetRedisConnectionString(string ip, string port, string password, int database, string masterName, bool isSentinelMode)
        {

            string connctionString;

            if (isSentinelMode)
            {
                masterName = string.IsNullOrWhiteSpace(masterName) ? "mymaster" : masterName;
                connctionString = $"{masterName},defaultDatabase={database},poolsize=50,connectTimeout=200,ssl=false,writeBuffer=10240,prefix=S2";
                if (!string.IsNullOrWhiteSpace(password))
                {
                    connctionString = $"{masterName},password={password},defaultDatabase={database},poolsize=50,connectTimeout=200,ssl=false,writeBuffer=10240,prefix=S2";
                }
            }
            else
            {
                connctionString = $"{ip}:{port},defaultDatabase={database},poolsize=50,ssl=false,writeBuffer=10240,prefix=S2";
                if (!string.IsNullOrWhiteSpace(password))
                {
                    connctionString = $"{ip}:{port},password={password},defaultDatabase={database},poolsize=50,ssl=false,writeBuffer=10240,prefix=S2";
                }
            }

            return connctionString;
        }
    }
}
