using RatableTracker.Rework.Util;
using RatableTracker.Rework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.LoadSave
{
    public class FileHandlerLocal : IFileHandler
    {
        protected readonly string baseDirectory;
        protected readonly IPathController pathController;

        public FileHandlerLocal(string baseDirectory, IPathController pathController)
        {
            this.baseDirectory = baseDirectory;
            this.pathController = pathController;
        }

        public byte[] LoadFile(string relativePath)
        {
            string fullPath = pathController.Combine(baseDirectory, relativePath);
            if (pathController.FileExists(fullPath))
                return Util.Util.TextEncoding.GetBytes(pathController.ReadFromFile(fullPath));
            else
                return new byte[0];
        }

        public void SaveFile(string relativePath, byte[] data)
        {
            string fullPath = pathController.Combine(baseDirectory, relativePath);
            pathController.WriteToFile(fullPath, Util.Util.TextEncoding.GetString(data));
        }

        public void AppendFile(string relativePath, byte[] data)
        {
            string fullPath = pathController.Combine(baseDirectory, relativePath);
            pathController.AppendToFile(fullPath, Util.Util.TextEncoding.GetString(data));
        }
    }
}
