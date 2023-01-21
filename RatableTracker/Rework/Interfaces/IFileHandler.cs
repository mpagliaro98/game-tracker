using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Interfaces
{
    public interface IFileHandler
    {
        byte[] LoadFile(string path, ILogger logger = null);
        void SaveFile(string path, byte[] data, ILogger logger = null);
        void AppendFile(string path, byte[] data, ILogger logger = null);
        void DeleteFile(string path, ILogger logger = null);
        IList<FileInfo> GetFilesInCurrentDirectory(ILogger logger = null);
    }
}
