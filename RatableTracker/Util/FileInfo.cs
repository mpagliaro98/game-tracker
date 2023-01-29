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

        public FileInfo(System.IO.FileInfo fileInfo) : this(fileInfo.Name + (fileInfo.Extension == "" ? "" : "." + fileInfo.Extension), fileInfo.CreationTimeUtc) { }

        protected FileInfo(string name, DateTime createdOnUTC)
        {
            Name = name;
            CreatedOnUTC = createdOnUTC;
        }
    }
}
