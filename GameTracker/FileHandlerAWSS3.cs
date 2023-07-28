using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.Runtime;
using Amazon.S3.Model;
using System.IO;
using Amazon;
using RatableTracker.Util;
using RatableTracker.Interfaces;
using System.Diagnostics;

namespace GameTracker
{
    public class FileHandlerAWSS3 : IFileHandler
    {
        private const string BUCKET_NAME_BASE = "gametrackersavefiles";
        private const string KEY_FILENAME = "awskeys.dat";

        private AWSCredentials credentials;
        private static readonly RegionEndpoint region = RegionEndpoint.USEast1;
        private string bucketName;
        private readonly IPathController pathController;

        private string KeyFilePath => pathController.Combine(pathController.ApplicationDirectory(), KEY_FILENAME);

        public FileHandlerAWSS3(IPathController pathController)
        {
            this.pathController = pathController;

            // load file, except if file not found, except if file is wrong format and delete file
            try
            {
                if (!KeyFileExists())
                    throw new FileNotFoundException("Key file does not exist. Call a different constructor to generate the key file.");
                ReadKeyFileAndCreateClient(KeyFilePath);
            }
            catch (Exception ex)
            {
                DeleteKeyFile();
                throw new FederatedAuthenticationFailureException("Error reading the credential key file.", ex);
            }
        }

        public FileHandlerAWSS3(string filepathContainingKeys, IPathController pathController)
        {
            this.pathController = pathController;

            // load incoming file and read keys out of it
            try
            {
                if (!this.pathController.FileExists(filepathContainingKeys))
                    throw new FileNotFoundException("Incoming file does not exist.");
                ReadKeyFileAndCreateClient(filepathContainingKeys);
            }
            catch (Exception ex)
            {
                DeleteKeyFile();
                throw new FederatedAuthenticationFailureException("Error reading incoming key file. Make sure it is formatted correctly.", ex);
            }
        }

        public FileHandlerAWSS3(string awsKey, string awsSecret, IPathController pathController)
        {
            this.pathController = pathController;

            // save key and secret to a file
            try
            {
                CreateKeyFile(KeyFilePath, awsKey, awsSecret);
                CreateCredentials(awsKey, awsSecret);
            }
            catch (Exception ex)
            {
                DeleteKeyFile();
                throw new FederatedAuthenticationFailureException("Error saving the credential key file.", ex);
            }
        }

        private void ReadKeyFileAndCreateClient(string keyFilePath)
        {
            string fileContents = pathController.ReadFromFile(keyFilePath);
            if (fileContents.Length <= 0)
                throw new Exception("Invalid AWS credentials");
            string[] fileLines = fileContents.Split('\n');
            if (fileLines.Length != 2)
                throw new Exception("Credential file is not formatted correctly");
            if (!KeyFileExists())
                CreateKeyFile(KeyFilePath, fileLines[0], fileLines[1]);
            CreateCredentials(fileLines[0], fileLines[1]);
        }

        private void CreateKeyFile(string path, string awsKey, string awsSecret)
        {
            pathController.CreateFileIfDoesNotExist(path);
            pathController.WriteToFile(path, awsKey + "\n" + awsSecret);
        }

        private void CreateCredentials(string awsKey, string awsSecret)
        {
            bucketName = BUCKET_NAME_BASE + "-" + awsKey.Trim().ToLower();
#if DEBUG
            bucketName = BUCKET_NAME_BASE + "-dev";
#endif

            credentials = new BasicAWSCredentials(awsKey.Trim(), awsSecret.Trim());
        }

        public bool KeyFileExists()
        {
            return pathController.FileExists(KeyFilePath);
        }

        public static bool KeyFileExists(IPathController pathController)
        {
            return pathController.FileExists(pathController.Combine(pathController.ApplicationDirectory(), KEY_FILENAME));
        }

        public void DeleteKeyFile()
        {
            pathController.DeleteFile(KeyFilePath);
        }

        public static void DeleteKeyFile(IPathController pathController)
        {
            pathController.DeleteFile(pathController.Combine(pathController.ApplicationDirectory(), KEY_FILENAME));
        }

        public void AppendFile(string keyName, byte[] data)
        {
            AppendFile(keyName, data, new Logger());
        }

        public void AppendFile(string keyName, byte[] data, Logger logger)
        {
            var fileContent = LoadFile(keyName, logger);
            fileContent = fileContent.Concat(data).ToArray();
            SaveFile(keyName, fileContent, logger);
        }

        public void DeleteFile(string keyName)
        {
            DeleteFile(keyName, new Logger());
        }

        public void DeleteFile(string keyName, Logger logger)
        {
            DeleteFromS3Bucket(bucketName, keyName, logger);
        }

        public IList<RatableTracker.Util.FileInfo> GetFilesInCurrentDirectory()
        {
            return GetFilesInCurrentDirectory(new Logger());
        }

        public IList<RatableTracker.Util.FileInfo> GetFilesInCurrentDirectory(Logger logger)
        {
            IList<S3Object> s3Objects = ListObjectsInS3Bucket(bucketName, logger);
            IList<RatableTracker.Util.FileInfo> files = new List<RatableTracker.Util.FileInfo>();
            foreach (var s3Object in s3Objects)
            {
                files.Add(new FileInfoAWSS3(s3Object));
            }
            return files;
        }

        public byte[] LoadFile(string keyName)
        {
            return LoadFile(keyName, new Logger());
        }

        public byte[] LoadFile(string keyName, Logger logger)
        {
            CreateBucketIfDoesNotExist(bucketName, logger);
            return RatableTracker.Util.Util.TextEncoding.GetBytes(ReadFromS3Bucket(bucketName, keyName, logger));
        }

        public void SaveFile(string keyName, byte[] data)
        {
            SaveFile(keyName, data, new Logger());
        }

        public void SaveFile(string keyName, byte[] data, Logger logger)
        {
            CreateBucketIfDoesNotExist(bucketName, logger);
            WriteToS3Bucket(bucketName, keyName, RatableTracker.Util.Util.TextEncoding.GetString(data), logger);
        }

        private string ReadFromS3Bucket(string bucketName, string keyName, Logger logger)
        {
            return ReadFromS3BucketAsync(bucketName, keyName, logger).GetAwaiter().GetResult();
        }

        private async Task<string> ReadFromS3BucketAsync(string bucketName, string keyName, Logger logger)
        {
            logger.Log("ReadFromS3BucketAsync - " + bucketName + " - " + keyName);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using var client = new AmazonS3Client(credentials, region);
            GetObjectRequest request = new()
            {
                BucketName = bucketName,
                Key = keyName
            };
            string content;
            try
            {
                GetObjectResponse response = await client.GetObjectAsync(request).ConfigureAwait(false);
                sw.Stop();
                logger.Log("GetObjectResponse received in " + sw.ElapsedMilliseconds.ToString() + "ms - " + response.HttpStatusCode.ToString() + " - len: " + response.ContentLength.ToString());
                StreamReader reader = new(response.ResponseStream);
                content = reader.ReadToEnd();
            }
            catch (AmazonS3Exception ex)
            {
                // Key does not exist yet
                content = "";
                sw.Stop();
                logger.Log("ReadFromS3BucketAsync ERROR - " + ex.GetType().Name + ": " + ex.Message + " (" + sw.ElapsedMilliseconds.ToString() + "ms)");
            }
            return content;
        }

        private void WriteToS3Bucket(string bucketName, string keyName, string content, Logger logger)
        {
            WriteToS3BucketAsync(bucketName, keyName, content, logger).GetAwaiter().GetResult();
        }

        private async Task WriteToS3BucketAsync(string bucketName, string keyName, string content, Logger logger)
        {
            logger.Log("WriteToS3BucketAsync - " + bucketName + " - " + keyName + " - len: " + content.Length.ToString());
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using var client = new AmazonS3Client(credentials, region);
            PutObjectRequest request = new()
            {
                BucketName = bucketName,
                Key = keyName,
                ContentBody = content
            };
            PutObjectResponse response = await client.PutObjectAsync(request).ConfigureAwait(false);
            sw.Stop();
            logger.Log("PutObjectRequest received in " + sw.ElapsedMilliseconds.ToString() + "ms - " + response.HttpStatusCode.ToString());
        }

        private bool DeleteFromS3Bucket(string bucketName, string keyName, Logger logger)
        {
            return DeleteFromS3BucketAsync(bucketName, keyName, logger).GetAwaiter().GetResult();
        }

        private async Task<bool> DeleteFromS3BucketAsync(string bucketName, string keyName, Logger logger)
        {
            logger.Log("DeleteFromS3BucketAsync - " + bucketName + " - " + keyName);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using var client = new AmazonS3Client(credentials, region);
            DeleteObjectRequest request = new()
            {
                BucketName = bucketName,
                Key = keyName
            };
            bool deleted;
            try
            {
                DeleteObjectResponse response = await client.DeleteObjectAsync(request).ConfigureAwait(false);
                sw.Stop();
                logger.Log("DeleteObjectResponse received in " + sw.ElapsedMilliseconds.ToString() + "ms - " + response.HttpStatusCode.ToString());
                deleted = response.HttpStatusCode.Equals(System.Net.HttpStatusCode.OK);
            }
            catch (AmazonS3Exception ex)
            {
                // Key does not exist yet
                deleted = false;
                sw.Stop();
                logger.Log("DeleteFromS3BucketAsync ERROR - " + ex.GetType().Name + ": " + ex.Message + " (" + sw.ElapsedMilliseconds.ToString() + "ms)");
            }
            return deleted;
        }

        private IList<S3Object> ListObjectsInS3Bucket(string bucketName, Logger logger)
        {
            return ListObjectsInS3BucketAsync(bucketName, logger).GetAwaiter().GetResult();
        }

        private async Task<IList<S3Object>> ListObjectsInS3BucketAsync(string bucketName, Logger logger)
        {
            logger.Log("ListObjectsInS3BucketAsync - " + bucketName);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using var client = new AmazonS3Client(credentials, region);
            ListObjectsV2Request request = new()
            {
                BucketName = bucketName,
                MaxKeys = 50
            };
            IList<S3Object> list;
            try
            {
                ListObjectsV2Response response = await client.ListObjectsV2Async(request).ConfigureAwait(false);
                sw.Stop();
                logger.Log("ListObjectsV2Response received in " + sw.ElapsedMilliseconds.ToString() + "ms - " + response.HttpStatusCode.ToString() + " - " + response.S3Objects.Count.ToString() + " objects found");
                list = response.S3Objects;
            }
            catch (AmazonS3Exception ex)
            {
                list = new List<S3Object>();
                sw.Stop();
                logger.Log("ListObjectsInS3BucketAsync ERROR - " + ex.GetType().Name + ": " + ex.Message + " (" + sw.ElapsedMilliseconds.ToString() + "ms)");
            }
            return list;
        }

        private void CreateBucketIfDoesNotExist(string bucketName, Logger logger)
        {
            CreateBucketIfDoesNotExistAsync(bucketName, logger).GetAwaiter().GetResult();
        }

        private async Task CreateBucketIfDoesNotExistAsync(string bucketName, Logger logger)
        {
            logger.Log("CreateBucketIfDoesNotExistAsync - " + bucketName);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using var client = new AmazonS3Client(credentials, region);
            ListBucketsResponse response = await client.ListBucketsAsync().ConfigureAwait(false);
            sw.Stop();
            logger.Log("ListBucketsResponse received in " + sw.ElapsedMilliseconds.ToString() + "ms - " + response.HttpStatusCode.ToString() + " - " + response.Buckets.Count.ToString() + " buckets found");
            bool found = false;
            foreach (S3Bucket bucket in response.Buckets)
            {
                if (bucket.BucketName == bucketName)
                {
                    logger.Log("Found " + bucketName);
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                logger.Log("Bucket not found, creating " + bucketName);
                sw.Restart();
                await client.PutBucketAsync(bucketName);
                sw.Stop();
                logger.Log("Bucket " + bucketName + " created in " + sw.ElapsedMilliseconds.ToString() + "ms");
            }
        }
    }
}
