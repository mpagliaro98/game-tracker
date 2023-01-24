using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Interfaces
{
    public interface IFileHandler
    {
        byte[] LoadFile(string path);
        byte[] LoadFile(string path, Logger logger);
        void SaveFile(string path, byte[] data);
        void SaveFile(string path, byte[] data, Logger logger);
        void AppendFile(string path, byte[] data);
        void AppendFile(string path, byte[] data, Logger logger);
        void DeleteFile(string path);
        void DeleteFile(string path, Logger logger);
        IList<FileInfo> GetFilesInCurrentDirectory();
        IList<FileInfo> GetFilesInCurrentDirectory(Logger logger);
    }
}
