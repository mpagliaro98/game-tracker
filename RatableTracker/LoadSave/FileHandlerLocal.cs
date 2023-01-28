using RatableTracker.Util;
using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RatableTracker.LoadSave
{
    public class FileHandlerLocal : IFileHandler
    {
        protected readonly static object dictLock = new();
        protected readonly static Dictionary<string, object> fileLocks = new();

        protected readonly string baseDirectory;
        protected readonly IPathController pathController;

        public FileHandlerLocal(string baseDirectory, IPathController pathController)
        {
            this.baseDirectory = baseDirectory;
            this.pathController = pathController;
        }

        public byte[] LoadFile(string relativePath)
        {
            return LoadFile(relativePath, new Logger());
        }

        public byte[] LoadFile(string relativePath, Logger logger)
        {
            string fullPath = pathController.Combine(baseDirectory, relativePath);
            logger.Log(GetType().Name + " LoadFile - attempting to read from " + fullPath);
            bool fileExists;
            byte[] bytes;
            lock (GetFileLock(fullPath))
            {
                if (pathController.FileExists(fullPath))
                {
                    bytes = Util.Util.TextEncoding.GetBytes(pathController.ReadFromFile(fullPath));
                    fileExists = true;
                }
                else
                {
                    bytes = Array.Empty<byte>();
                    fileExists = false;
                }
            }
            if (fileExists)
                logger.Log("Found " + bytes.Length.ToString() + " bytes at " + fullPath);
            else
                logger.Log("No file found at " + fullPath + ", returning 0 bytes");
            return bytes;
        }

        public void SaveFile(string relativePath, byte[] data)
        {
            SaveFile(relativePath, data, new Logger());
        }

        public void SaveFile(string relativePath, byte[] data, Logger logger)
        {
            string fullPath = pathController.Combine(baseDirectory, relativePath);
            logger.Log(GetType().Name + " SaveFile - writing " + data.Length.ToString() + " bytes to " + fullPath);
            lock (GetFileLock(fullPath))
            {
                pathController.CreateFileIfDoesNotExist(fullPath);
                pathController.WriteToFile(fullPath, Util.Util.TextEncoding.GetString(data));
            }
        }

        public void AppendFile(string relativePath, byte[] data)
        {
            AppendFile(relativePath, data, new Logger());
        }

        public void AppendFile(string relativePath, byte[] data, Logger logger)
        {
            string fullPath = pathController.Combine(baseDirectory, relativePath);
            logger.Log(GetType().Name + " AppendFile - appending " + data.Length.ToString() + " bytes to " + fullPath);
            lock (GetFileLock(fullPath))
            {
                pathController.CreateFileIfDoesNotExist(fullPath);
                pathController.AppendToFile(fullPath, Util.Util.TextEncoding.GetString(data));
            }
        }

        public void DeleteFile(string relativePath)
        {
            DeleteFile(relativePath, new Logger());
        }

        public void DeleteFile(string relativePath, Logger logger)
        {
            string fullPath = pathController.Combine(baseDirectory, relativePath);
            logger.Log(GetType().Name + " DeleteFile - deleting " + fullPath);
            lock (GetFileLock(fullPath))
                pathController.DeleteFile(fullPath);
        }

        public IList<Util.FileInfo> GetFilesInCurrentDirectory()
        {
            return GetFilesInCurrentDirectory(new Logger());
        }

        public IList<Util.FileInfo> GetFilesInCurrentDirectory(Logger logger)
        {
            pathController.CreateDirectory(baseDirectory);
            var dir = new DirectoryInfo(baseDirectory);
            IList<Util.FileInfo> files = new List<Util.FileInfo>();
            logger.Log(GetType().Name + " GetFilesInCurrentDirectory - found " + files.Count.ToString() + " files in " + baseDirectory);
            foreach (var file in dir.GetFiles())
            {
                files.Add(new Util.FileInfo(file));
            }
            return files;
        }

        protected static object GetFileLock(string fullPath)
        {
            lock (dictLock)
            {
                if (fileLocks.TryGetValue(fullPath, out object value))
                    return value;
                else
                {
                    object newLock = new();
                    fileLocks[fullPath] = newLock;
                    return newLock;
                }
            }
        }
    }
}
