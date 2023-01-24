using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.LoadSave
{
    public class PathControllerWindows : IPathController
    {
        public virtual string ApplicationDirectory()
        {
            return Environment.ExpandEnvironmentVariables("%LocalAppData%\\mpagliaro98\\GameTracker");
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
            using (StreamWriter sw = new StreamWriter(filepath, false))
                await sw.WriteAsync(text);
        }

        public string ReadFromFile(string filepath)
        {
            return System.IO.File.ReadAllText(filepath);
        }

        public async Task<string> ReadFromFileAsync(string filepath)
        {
            string text;
            using (StreamReader reader = File.OpenText(filepath))
                text = await reader.ReadToEndAsync();
            return text;
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
            if (!FileExists(filepath))
            {
                string directory = GetDirectoryFromFilename(filepath);
                CreateDirectory(directory);
                System.IO.FileStream fs = CreateFile(filepath);
                fs.Close();
            }
        }

        public void DeleteFile(string filepath)
        {
            File.Delete(filepath);
        }

        public void AppendToFile(string filepath, string text)
        {
            using (StreamWriter sw = new StreamWriter(filepath, true))
                sw.Write(text);
        }

        public async Task AppendToFileAsync(string filepath, string text)
        {
            using (StreamWriter sw = new StreamWriter(filepath, true))
                await sw.WriteAsync(text);
        }
    }
}
