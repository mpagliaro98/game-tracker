using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.IO
{
    public class ContentLoadSaveTransferString : ContentLoadSaveTransferBase<string, string, string, string>
    {
        public override void TransferSaveFiles(IContentLoadSave<string, string> from,
            IContentLoadSave<string, string> to, IEnumerable<string> keys)
        {
            foreach (string key in keys)
            {
                string content = from.Read(key);
                to.Write(key, content);
            }
        }

        public override async Task TransferSaveFilesAsync(IContentLoadSave<string, string> from,
            IContentLoadSave<string, string> to, IEnumerable<string> keys)
        {
            foreach (string key in keys)
            {
                string content = await from.ReadAsync(key);
                await to.WriteAsync(key, content);
            }
        }
    }
}
