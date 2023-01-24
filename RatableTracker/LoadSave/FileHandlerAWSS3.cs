using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.LoadSave
{
    public class FileHandlerAWSS3 : IFileHandler
    {
        public FileHandlerAWSS3(string awsKey, string awsSecret)
        {

        }

        public void AppendFile(string path, byte[] data)
        {
            throw new NotImplementedException();
        }

        public void AppendFile(string path, byte[] data, Logger logger)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string path)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string path, Logger logger)
        {
            throw new NotImplementedException();
        }

        public IList<FileInfo> GetFilesInCurrentDirectory()
        {
            throw new NotImplementedException();
        }

        public IList<FileInfo> GetFilesInCurrentDirectory(Logger logger)
        {
            throw new NotImplementedException();
        }

        public byte[] LoadFile(string path)
        {
            throw new NotImplementedException();
        }

        public byte[] LoadFile(string path, Logger logger)
        {
            throw new NotImplementedException();
        }

        public void SaveFile(string path, byte[] data)
        {
            throw new NotImplementedException();
        }

        public void SaveFile(string path, byte[] data, Logger logger)
        {
            throw new NotImplementedException();
        }
    }
}
