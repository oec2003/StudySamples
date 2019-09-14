using System;
using System.Threading;

namespace RedisLockLib
{
    public class SeqNo
    {
        private readonly static object _myLock = new object();
        private static CSRedis.CSRedisClient _redisClient=null;

        public static string GetSeqNoNoLock()
        {
            string connectionStr = "server = localhost; user id = sqluser; password = Pass@word; database = seqno_test";
            string getSeqNosql = "select num from seqno where code='order'";
            string updateSeqNoSql = "update seqno set num=num+1 where code='order'";

            var seqNo = MySQLHelper.ExecuteScalar(connectionStr, System.Data.CommandType.Text, getSeqNosql);
            MySQLHelper.ExecuteNonQuery(connectionStr, System.Data.CommandType.Text, updateSeqNoSql);

            return seqNo.ToString();
        }

        public static string GetSeqNoByLock()
        {
            string connectionStr = "server = localhost; user id = sqluser; password = Pass@word; database = seqno_test";
            string getSeqNosql = "select num from seqno where code='order'";
            string updateSeqNoSql = "update seqno set num=num+1 where code='order'";
            var seqNo = string.Empty;
            try
            {
                Monitor.Enter(_myLock);
                seqNo = MySQLHelper.ExecuteScalar(connectionStr, System.Data.CommandType.Text, getSeqNosql).ToString();

                MySQLHelper.ExecuteNonQuery(connectionStr, System.Data.CommandType.Text, updateSeqNoSql);

                Monitor.Exit(_myLock);
            }
            catch
            {
                Monitor.Exit(_myLock);
            }

            return seqNo.ToString();
        }

        public static string GetSeqNoByRedisLock()
        {
            string connectionStr = "server = localhost; user id = sqluser; password = Pass@word; database = seqno_test";
            string getSeqNosql = "select num from seqno where code='order'";
            string updateSeqNoSql = "update seqno set num=num+1 where code='order'";

            var seqNo=string.Empty;
            using (_redisClient.Lock("test", 5000))
            {
                seqNo = MySQLHelper.ExecuteScalar(connectionStr, System.Data.CommandType.Text, getSeqNosql).ToString();

                MySQLHelper.ExecuteNonQuery(connectionStr, System.Data.CommandType.Text, updateSeqNoSql);
            }
            return seqNo;
        }

        public static void InitRedis()
        {
            _redisClient = new CSRedis.CSRedisClient("localhost:6379,password=,defaultDatabase=8,poolsize=50,ssl=false,writeBuffer=10240,prefix=oec2003");
        }
    }
}
