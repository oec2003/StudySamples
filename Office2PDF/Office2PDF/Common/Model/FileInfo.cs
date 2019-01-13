using System;
using System.Collections.Generic;
using System.Text;

namespace Office2PDF.Common
{
    [Serializable()]
    public class FileInfo
    {
        public virtual string FileName { get; set; }
        public virtual string FileId { get; set; }
        public virtual string Md5 { get; set; }

        public virtual long Length { get; set; }

        public virtual DateTime UploadDateTime { get; set; }
    }
}
