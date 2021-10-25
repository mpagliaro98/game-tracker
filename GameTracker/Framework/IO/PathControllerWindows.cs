﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.IO
{
    public class PathControllerWindows : IPathController
    {
        public string BaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public string Combine(string path1, string path2)
        {
            return System.IO.Path.Combine(path1, path2);
        }

        public void WriteToFile(string filepath, string text)
        {
            System.IO.File.WriteAllText(filepath, text);
        }

        public async Task WriteToFileAsync(string filepath, string text)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            using (StreamWriter sw = new StreamWriter(fs))
                await sw.WriteAsync(text);
        }

        public string ReadFromFile(string filepath)
        {
            return System.IO.File.ReadAllText(filepath);
        }

        public async Task<string> ReadFromFileAsync(string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
                return await sr.ReadToEndAsync();
        }

        public bool FileExists(string filepath)
        {
            return System.IO.File.Exists(filepath);
        }

        public string GetDirectoryFromFilename(string filepath)
        {
            return System.IO.Path.GetDirectoryName(filepath);
        }

        public System.IO.DirectoryInfo CreateDirectory(string directory)
        {
            return System.IO.Directory.CreateDirectory(directory);
        }

        public System.IO.FileStream CreateFile(string filepath)
        {
            return System.IO.File.Create(filepath);
        }

        public void CreateFileIfDoesNotExist(string filepath)
        {
            if (!PathController.FileExists(filepath))
            {
                string directory = PathController.GetDirectoryFromFilename(filepath);
                PathController.CreateDirectory(directory);
                System.IO.FileStream fs = PathController.CreateFile(filepath);
                fs.Close();
            }
        }
    }
}
