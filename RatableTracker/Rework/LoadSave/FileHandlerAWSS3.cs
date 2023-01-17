using RatableTracker.Rework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.LoadSave
{
    public class FileHandlerAWSS3 : IFileHandler
    {
        public FileHandlerAWSS3(string awsKey, string awsSecret)
        {

        }

        public byte[] LoadFile(string path)
        {
            throw new NotImplementedException();
        }

        public void SaveFile(string path, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
