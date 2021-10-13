using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public interface IPathController
    {
        string BaseDirectory();
        string Combine(string path1, string path2);
        void WriteToFile(string filepath, string text);
        string ReadFromFile(string filepath);
        bool FileExists(string filepath);
        string GetDirectoryFromFilename(string filepath);
        System.IO.DirectoryInfo CreateDirectory(string directory);
        System.IO.FileStream CreateFile(string filepath);
    }
}
