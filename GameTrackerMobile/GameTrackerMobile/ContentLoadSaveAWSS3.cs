using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using Amazon.S3;
using Amazon.Runtime;
using Amazon.S3.Model;
using System.IO;
using Amazon;
using Amazon.S3.Transfer;
using System.Threading;
using System.Diagnostics;

namespace RatableTracker.Framework.IO
{
    public class ContentLoadSaveAWSS3 : IContentLoadSave<string, string>
    {
        private static string BUCKET_NAME = "gametrackersavefiles";
        private const string KEY_FILENAME = "awskeys.dat";
        private static string KEY_FILE_PATH = PathController.Combine(PathController.BaseDirectory(), KEY_FILENAME);

        public TransferUtility s3transferUtility;
        IAmazonS3 s3client;

        public ContentLoadSaveAWSS3()
        {


            Debug.WriteLine("Load save started");
            string fileContents = IO.PathController.ReadFromFile(KEY_FILE_PATH);
            if (fileContents.Length <= 0)
                throw new Exception("Invalid AWS credentials");
            string[] fileLines = fileContents.Split('\n');
            if (fileLines.Length != 2)
                throw new Exception("Credentials were not formatted correctly");

            string clientId = fileLines[0].Trim();
            string clientSecret = fileLines[1].Trim();
            Debug.WriteLine(clientId + "\n" + clientSecret);

            BUCKET_NAME = "gametrackersavefiles-" + clientId.ToLower();
#if DEBUG
            BUCKET_NAME = "gametrackersavefiles-dev";
#endif

            var config = new AmazonS3Config() { RegionEndpoint = Amazon.RegionEndpoint.USEast1, Timeout = TimeSpan.FromSeconds(30), UseHttp = true };
            this.s3client = new AmazonS3Client(clientId, clientSecret, config);
            Debug.WriteLine("Client created");
            
            AWSConfigsS3.UseSignatureVersion4 = true;
            this.s3transferUtility = new TransferUtility(s3client);
            Debug.WriteLine("Transfer utility created");
        }

        public string Read(string key)
        {
            Debug.WriteLine("Starting read: " + key);
            CreateBucketIfDoesNotExist(BUCKET_NAME);
            string content = Task.Run(async() => await ReadFromS3BucketAsync(BUCKET_NAME, key)).Result;
            return content;
        }

        public async Task<string> ReadAsync(string key)
        {
            Debug.WriteLine("Starting read async: " + key);
            CreateBucketIfDoesNotExist(BUCKET_NAME);
            string content = await ReadFromS3BucketAsync(BUCKET_NAME, key);
            return content;
        }

        public void Write(string key, string output)
        {
            Debug.WriteLine("Starting write: " + key);
            CreateBucketIfDoesNotExist(BUCKET_NAME);
            Task.Run(async () => { await WriteToS3BucketAsync(BUCKET_NAME, key, output); }).Wait();
        }

        public async Task WriteAsync(string key, string output)
        {
            Debug.WriteLine("Starting write async: " + key);
            CreateBucketIfDoesNotExist(BUCKET_NAME);
            await WriteToS3BucketAsync(BUCKET_NAME, key, output);
        }

        private async Task<string> ReadFromS3BucketAsync(string bucketName, string keyName)
        {
            try
            {
                string tempPath = PathController.Combine(PathController.BaseDirectory(), keyName);

                TransferUtilityDownloadRequest request = new TransferUtilityDownloadRequest
                {
                    BucketName = bucketName,
                    FilePath = tempPath,
                    Key = keyName
                };

                Debug.WriteLine("Before download to " + tempPath);
                try
                {
                    Task.Run(async () => { await this.s3transferUtility.DownloadAsync(request); }).Wait();
                }
                catch (Exception e)
                {
                    // key doesn't exist
                    Debug.WriteLine("EXCEPTION: " + e.GetType().Name + " - " + e.Message);
                    if (e.InnerException != null && e.InnerException is AmazonS3Exception && e.InnerException.Message.ToLower().StartsWith("the specified key does not exist"))
                    {
                        Debug.WriteLine("Creating empty file for " + keyName);
                        await WriteToS3BucketAsync(bucketName, keyName, "");
                    }
                    return "";
                }

                Debug.WriteLine("Downloaded, reading from local temp file");
                string content = await PathController.ReadFromFileAsync(tempPath);
                PathController.DeleteFile(tempPath);
                return content;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task WriteToS3BucketAsync(string bucketName, string keyName, string content)
        {
            try
            {
                string tempPath = PathController.Combine(PathController.BaseDirectory(), "temp.json");
                await PathController.WriteToFileAsync(tempPath, content);

                TransferUtilityUploadRequest request =
                    new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        FilePath = tempPath,
                        Key = keyName,
                        ContentType = "application/json"
                    };

                await this.s3transferUtility.UploadAsync(request).ContinueWith(((x) =>
                {
                    Debug.WriteLine("Uploaded");
                }));

                PathController.DeleteFile(tempPath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CreateBucketIfDoesNotExist(string bucketName)
        {
            ListBucketsResponse response = this.s3transferUtility.S3Client.ListBucketsAsync().Result; 
            bool found = false;
            foreach (S3Bucket bucket in response.Buckets)
            {
                if (bucket.BucketName == bucketName)
                {
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                Debug.WriteLine("No bucket found, creating bucket " + bucketName);
                Task.Run(async () => { await this.s3transferUtility.S3Client.PutBucketAsync(bucketName); }).Wait();
                Debug.WriteLine("Bucket created");
            }
        }

        public static bool KeyFileExists()
        {
            return PathController.FileExists(KEY_FILE_PATH);
        }

        public static void CreateKeyFile(string keyFilePath)
        {
            PathController.CreateFileIfDoesNotExist(KEY_FILE_PATH);
            string fileContents = PathController.ReadFromFile(keyFilePath);
            PathController.WriteToFile(KEY_FILE_PATH, fileContents);
        }

        public static void DeleteKeyFile()
        {
            PathController.DeleteFile(KEY_FILE_PATH);
        }
    }
}
