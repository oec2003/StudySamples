using System;
using System.IO;
using MongoDB.Driver.GridFS;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Configuration;

namespace Office2PDF.Common
{
    public class GridFSOperationManager : IFileOperationManager
    {
        private  IMongoDatabase _db;
        private  string _dbName;
        private  string _hostName;
        private  int _port;
        private  int _chunkSize;
        private  GridFSBucket _gridFs;
        
        public GridFSOperationManager()
        {
            Init();
        }

        private void Init()
        {
            _hostName = ConfigurationManager.AppSettings["MongoHostName"];
            _dbName = ConfigurationManager.AppSettings["MongoDBName"];

            int.TryParse((ConfigurationManager.AppSettings["MongoPort"] + string.Empty), out _port);
            int.TryParse((ConfigurationManager.AppSettings["MongoChunkSize"] + string.Empty), out _chunkSize);//261120
            
            List<MongoServerAddress> mongoServerAddress = new List<MongoServerAddress>();
            if (!string.IsNullOrEmpty(_hostName))
            {
                foreach (string address in _hostName.Split(','))
                {
                    mongoServerAddress.Add(new MongoServerAddress(address, _port));
                }

                MongoClientSettings mcs = new MongoClientSettings();
                mcs.Servers = mongoServerAddress;
                MongoClient mc = new MongoClient(mcs);

                _db = mc.GetDatabase(_dbName);
                _gridFs = new GridFSBucket(_db);
            }
            else
            {
                Console.WriteLine("Can't find MongoHostName!!!");
            }
        }


        private FileInfo AddFile(Stream fileStream, string fileName)
        {
            var briefInfo = new FileInfo();
            GridFSUploadOptions options = new GridFSUploadOptions();
            options.ChunkSizeBytes = _chunkSize;
            ObjectId id = _gridFs.UploadFromStream(fileName, fileStream, options);
            briefInfo = GetFileInfo(id.ToString());

            return briefInfo;
        }

        private static byte[] StreamToBytes(Stream stream)
        {
            if (stream == null)
            {
                return null;
            }
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        public string GetMD5Hash(Stream stream)
        {
            string result = "";
            string hashData = "";
            byte[] arrbytHashValue;
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher =
                       new System.Security.Cryptography.MD5CryptoServiceProvider();

            try
            {
                arrbytHashValue = md5Hasher.ComputeHash(stream);//计算指定Stream 对象的哈希值
                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
                hashData = System.BitConverter.ToString(arrbytHashValue);
                //替换-
                hashData = hashData.Replace("-", "");
                result = hashData;
            }
            catch (System.Exception ex)
            {
                //记录日志
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public FileInfo AddFile(Stream fileStream, string fileName, bool isCheckExist=true)
        {
            if (!isCheckExist)
            {
                return AddFile(fileStream, fileName);
            }

            var result = new FileInfo();
            byte[] bytes = StreamToBytes(fileStream);
            //如果md5相同 着不需要再上传
            string md5 = GetMD5Hash(fileStream);
            string id = IsExist(md5.ToLower());
            if (string.IsNullOrEmpty(id))
            {
                var oldStream = BytesToStream(bytes);
                result = AddFile(oldStream, fileName);
            }
            else
            {
                result = GetFileInfo(id);
            }
            return result;
        }

        public bool DeleteFile(string fileId)
        {
            _gridFs.Delete(ObjectId.Parse(fileId));
            return true;
        }

        public string IsExist(string tag)
        {
            string id = "";
            GridFSFindOptions options = new GridFSFindOptions();
            options.Limit = 1;
            var filter = MongoDB.Driver.Builders<GridFSFileInfo>.Filter.Eq("md5", tag);
            var filterDefintion = FilterDefinition<GridFSFileInfo>.Empty;

            var objFile = _gridFs.Find(filter, options);
            if (objFile == null)
                return id;
            var files = objFile.ToList();

            if (files.Count > 0)
            {
                id = files.FirstOrDefault().Id.ToString();
            }
            return id;
        }

        public Stream GetFile(string fileId)
        {
            //ObjectId id = ObjectId.Parse(fileId);
            //byte[] fileByte =  _gridFs.DownloadAsBytes(id);
            //return BytesToStream(fileByte);
            return GetStream(fileId);
        }
        public Stream GetStream(string fileId)
        {
            try
            {
                ObjectId id = ObjectId.Parse(fileId);
                return _gridFs.OpenDownloadStream(id, new GridFSDownloadOptions()
                {
                    Seekable = true
                });
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public string GetFileName(string fileId)
        {
            ObjectId id = ObjectId.Parse(fileId);
            var fileInfo = GetGridFSFileInfo(id);
            if (fileInfo != null)
            {
                return fileInfo.Filename;
            }
            return string.Empty;
        }

        private GridFSFileInfo GetGridFSFileInfo(ObjectId id)
        {
            var filter = MongoDB.Driver.Builders<GridFSFileInfo>.Filter.Eq("_id", id);
            var fileInfo = _gridFs.Find(filter).ToList();
            return fileInfo.FirstOrDefault();
        }

        public string GetMD5(string fileId)
        {
            ObjectId id = ObjectId.Parse(fileId);
            var fileInfo = GetGridFSFileInfo(id);
            if (fileInfo != null)
            {
                return fileInfo.MD5;
            }
            return string.Empty;
        }


        public FileInfo GetFileInfo(string fileId)
        {
            var result = new FileInfo();
            var fileInfo = GetGridFSFileInfo(ObjectId.Parse(fileId));
            if (fileInfo != null)
            {
                result.Md5 = fileInfo.MD5;
                result.FileId = fileInfo.Id.ToString();
                result.Length = fileInfo.Length;
                result.UploadDateTime = fileInfo.UploadDateTime;
                result.FileName = fileInfo.Filename;
            }
            return result;
        }
        
        public FileByteInfo DownloadFile(string fileId)
        {
            FileByteInfo retFile = new FileByteInfo();
            retFile.Bytes= StreamToBytes(this.GetFile(fileId));

            if (retFile == null || retFile.Bytes == null)
                throw new GridFSFileNotFoundException(fileId);

            retFile.FileName = GetFileName(fileId);
            retFile.ContentType = Path.GetExtension(retFile.FileName).ToLower();
            return retFile;
        }
    }
}
