using System;
using System.Collections.Generic;
using System.IO;
namespace Office2PDF.Common
{
    public interface IFileOperationManager
    {
        FileInfo AddFile(Stream fileStream, string fileName, bool isCheckExist=true);
        FileByteInfo DownloadFile(string fileId);
        bool DeleteFile(string fileId);
        string IsExist(string tag);
        Stream GetFile(string fileId);
        Stream GetStream(string fileId);
        string GetFileName(string fileId);

        string GetMD5(string fileId);

        FileInfo GetFileInfo(string fileId);
    }
}
