using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Util
{
    public class FileInfo
    {
        public string Name { get; protected set; }
        public DateTime CreatedOnUTC { get; protected set; }

        public FileInfo(System.IO.FileInfo fileInfo)
        {
            Name = fileInfo.Name + (fileInfo.Extension == "" ? "" : "." + fileInfo.Extension);
            CreatedOnUTC = fileInfo.CreationTimeUtc;
        }
    }
}
