using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.LoadSave
{
    public class FileHandlerAWSS3 : IFileHandler
    {
        protected ILogger _logger;

        public FileHandlerAWSS3(string awsKey, string awsSecret, ILogger logger = null)
        {
            _logger = logger;
        }

        public byte[] LoadFile(string bucketID, ILogger logger = null)
        {
            throw new NotImplementedException();
        }

        public void SaveFile(string bucketID, byte[] data, ILogger logger = null)
        {
            throw new NotImplementedException();
        }

        public void AppendFile(string bucketID, byte[] data, ILogger logger = null)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string path, ILogger logger = null)
        {
            throw new NotImplementedException();
        }

        public IList<FileInfo> GetFilesInCurrentDirectory(ILogger logger = null)
        {
            throw new NotImplementedException();
        }
    }
}
