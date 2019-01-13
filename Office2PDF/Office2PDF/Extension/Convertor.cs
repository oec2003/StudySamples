using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Office2PDF.Extension
{
    public class Convertor
    {
        /// <summary>
        /// 二进制反序列化：byte=>实体object
        /// </summary>
        /// <param name="SerializedObj"></param>
        /// <param name="ThrowException"></param>
        /// <returns></returns>
        public static object ByteArrayToObject(byte[] SerializedObj, bool ThrowException)
        {
            if (SerializedObj == null)
            {
                return null;
            }
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream serializationStream = new MemoryStream(SerializedObj);
                return formatter.Deserialize(serializationStream);
            }
            catch (Exception exception)
            {
                if (ThrowException)
                {
                    throw exception;
                }
                return null;
            }
        }

        /// <summary>
        /// 二进制序列化：实体=》byte
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="ThrowException"></param>
        /// <returns></returns>
        public static byte[] ObjectToByteArray(object Obj, bool ThrowException)
        {
            if (Obj == null)
            {
                return null;
            }
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream serializationStream = new MemoryStream();
                formatter.Serialize(serializationStream, Obj);
                return serializationStream.ToArray();
            }
            catch (Exception exception)
            {
                if (ThrowException)
                {
                    throw exception;
                }
                return null;
            }
        }
    }
}
