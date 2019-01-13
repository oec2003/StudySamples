using System;
using System.Collections.Generic;
using System.Text;
using Office2PDF.MQ;
using Office2PDF.Messages;
using System.Diagnostics;
using System.IO;
using System.Configuration;

namespace Office2PDF.Common
{
    public class PowerPointConverter : IPowerPointConverter
    {
        IFileOperationManager fileOperation;

        public PowerPointConverter()
        {
            fileOperation = new GridFSOperationManager();
        }

        private bool SaveToFile(Stream stream,string path)
        {
            try
            {
                byte[] sourceBuffer = new Byte[stream.Length];
                stream.Read(sourceBuffer, 0, sourceBuffer.Length);
                stream.Seek(0, SeekOrigin.Begin);

                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(sourceBuffer, 0, sourceBuffer.Length);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取PPT源文件到本地失败：" + ex.Message);
                return false;
            }
            return true;
        }

        private FileInfo UploadFile(string path,string destName)
        {
            if (!File.Exists(path))
                return null;

            byte[] bytes = null;
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
            }

            if (bytes == null)
                return null;
            Stream stream = new MemoryStream(bytes);
            return fileOperation.AddFile(stream, destName, true);
        }
        public bool OnWork(MQ.Messages Message)
        {
            PowerPointConvertMessage message = (PowerPointConvertMessage)Message;
            string sourcePath = string.Empty;
            string destPath = string.Empty;
            try
            {
                if(message == null)
                    return false;
                Stream sourceStream = fileOperation.GetFile(message.FileInfo.FileId);
                string filename = message.FileInfo.FileId;
                string extension = System.IO.Path.GetExtension(message.FileInfo.FileName);
                sourcePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), filename + extension);
                destPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), string.Format("{0}.pdf", filename));

                if (!SaveToFile(sourceStream, sourcePath))
                    return false;
                var psi = new ProcessStartInfo("libreoffice", string.Format("--invisible --convert-to pdf  {0}", filename + extension)) { RedirectStandardOutput = true };
                // 启动
                var proc = Process.Start(psi);
                if (proc == null)
                {
                    Console.WriteLine("不能执行.");
                    return false;
                }
                else
                {
                    Console.WriteLine("-------------开始执行--------------");
                    //开始读取
                    using (var sr = proc.StandardOutput)
                    {
                        while (!sr.EndOfStream)
                        {
                            Console.WriteLine(sr.ReadLine());
                        }
                        if (!proc.HasExited)
                        {
                            proc.Kill();
                        }
                    }
                    Console.WriteLine("---------------执行完成------------------");
                    Console.WriteLine($"退出代码 ： {proc.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (File.Exists(destPath))
                {
                    var destFileInfo = UploadFile(destPath, string.Format("{0}.pdf", Path.GetFileNameWithoutExtension(message.FileInfo.FileName)));
                }
                if (File.Exists(destPath))
                {
                    System.IO.File.Delete(destPath);
                }
            }
            return true;
        }
    }
}
