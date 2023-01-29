using Amazon.S3.Model;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerWPF
{
    public class FileInfoAWSS3 : FileInfo
    {
        public FileInfoAWSS3(S3Object s3Object) : base(s3Object.Key, s3Object.LastModified) { }
    }
}
