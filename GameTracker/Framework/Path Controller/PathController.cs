using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.PathController
{
    public static class PathController
    {
        public static IPathController PathControllerInstance { get; set; }

        public static string BaseDirectory()
        {
            return PathControllerInstance.BaseDirectory();
        }

        public static string Combine(string path1, string path2)
        {
            return PathControllerInstance.Combine(path1, path2);
        }

        public static void WriteToFile(string filepath, string text)
        {
            PathControllerInstance.WriteToFile(filepath, text);
        }

        public static string ReadFromFile(string filepath)
        {
            return PathControllerInstance.ReadFromFile(filepath);
        }

        public static bool FileExists(string filepath)
        {
            return PathControllerInstance.FileExists(filepath);
        }

        public static string GetDirectoryFromFilename(string filepath)
        {
            return PathControllerInstance.GetDirectoryFromFilename(filepath);
        }

        public static System.IO.DirectoryInfo CreateDirectory(string directory)
        {
            return PathControllerInstance.CreateDirectory(directory);
        }

        public static System.IO.FileStream CreateFile(string filepath)
        {
            return PathControllerInstance.CreateFile(filepath);
        }
    }
}
