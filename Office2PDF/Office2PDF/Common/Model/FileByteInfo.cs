using System;
namespace Office2PDF.Common
{
    public class FileByteInfo
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public Byte[] Bytes { get; set; }
    }
}