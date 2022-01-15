using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.IO
{
    public static class ContentLoadSave
    {
        public static IContentLoadSave<string, string> ContentLoadSaveInstance { get; set; } = new ContentLoadSaveLocal();

        public static string Read(string key)
        {
            string content = ContentLoadSaveInstance.Read(key);
            System.Diagnostics.Debug.WriteLine("Read " + Encoding.UTF8.GetByteCount(content).ToString() + " bytes from " + key);
            return content;
        }

        public static async Task<string> ReadAsync(string key)
        {
            string content = await ContentLoadSaveInstance.ReadAsync(key);
            System.Diagnostics.Debug.WriteLine("Read " + Encoding.UTF8.GetByteCount(content).ToString() + " bytes from " + key);
            return content;
        }

        public static void Write(string key, string output)
        {
            ContentLoadSaveInstance.Write(key, output);
            System.Diagnostics.Debug.WriteLine("Wrote " + Encoding.UTF8.GetByteCount(output).ToString() + " bytes to " + key);
        }

        public static async Task WriteAsync(string key, string output)
        {
            await ContentLoadSaveInstance.WriteAsync(key, output);
            System.Diagnostics.Debug.WriteLine("Wrote " + Encoding.UTF8.GetByteCount(output).ToString() + " bytes to " + key);
        }

        public static void TransferSaveFiles(IContentLoadSave<string, string> from,
            IContentLoadSave<string, string> to, IEnumerable<string> keys)
        {
            ContentLoadSaveTransferString transfer = new ContentLoadSaveTransferString();
            transfer.TransferSaveFiles(from, to, keys);
        }

        public static async Task TransferSaveFilesAsync(IContentLoadSave<string, string> from,
            IContentLoadSave<string, string> to, IEnumerable<string> keys)
        {
            ContentLoadSaveTransferString transfer = new ContentLoadSaveTransferString();
            await transfer.TransferSaveFilesAsync(from, to, keys);
        }
    }
}
