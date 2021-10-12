using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    public static class PathController
    {
        private static IPathController GetPCInstance()
        {
            return new PathControllerWPF();
        }

        public static string BaseDirectory()
        {
            IPathController pc = GetPCInstance();
            return pc.BaseDirectory();
        }

        public static string Combine(string path1, string path2)
        {
            IPathController pc = GetPCInstance();
            return pc.Combine(path1, path2);
        }

        public static void WriteToFile(string filepath, string text)
        {
            IPathController pc = GetPCInstance();
            pc.WriteToFile(filepath, text);
        }

        public static string ReadFromFile(string filepath)
        {
            IPathController pc = GetPCInstance();
            return pc.ReadFromFile(filepath);
        }

        public static bool FileExists(string filepath)
        {
            IPathController pc = GetPCInstance();
            return pc.FileExists(filepath);
        }

        public static string GetDirectoryFromFilename(string filepath)
        {
            IPathController pc = GetPCInstance();
            return pc.GetDirectoryFromFilename(filepath);
        }

        public static System.IO.DirectoryInfo CreateDirectory(string directory)
        {
            IPathController pc = GetPCInstance();
            return pc.CreateDirectory(directory);
        }

        public static System.IO.FileStream CreateFile(string filepath)
        {
            IPathController pc = GetPCInstance();
            return pc.CreateFile(filepath);
        }
    }
}
