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

namespace RatableTracker.Framework.IO
{
    public class ContentLoadSaveAWSS3 : IContentLoadSave<string, string>
    {
        private const string BUCKET_NAME = "gametrackersavefiles";
        private const string KEY_FILENAME = "awskeys.dat";
        private static string KEY_FILE_PATH = PathController.Combine(PathController.BaseDirectory(), KEY_FILENAME);

        private readonly AmazonS3Client client;

        public ContentLoadSaveAWSS3()
        {
            string fileContents = IO.PathController.ReadFromFile(KEY_FILE_PATH);
            if (fileContents.Length <= 0)
                throw new FederatedAuthenticationFailureException("Invalid AWS credentials");
            string[] fileLines = fileContents.Split('\n');
            if (fileLines.Length != 2)
                throw new FederatedAuthenticationFailureException("Credentials were not formatted correctly");
            AWSCredentials credentials = new BasicAWSCredentials(fileLines[0].Trim(), fileLines[1].Trim());
            client = new AmazonS3Client(credentials, RegionEndpoint.USEast1);
        }

        public string Read(string key)
        {
            CreateBucketIfDoesNotExist(BUCKET_NAME);
            return ReadFromS3Bucket(BUCKET_NAME, key);
        }

        public async Task<string> ReadAsync(string key)
        {
            CreateBucketIfDoesNotExist(BUCKET_NAME);
            return await ReadFromS3BucketAsync(BUCKET_NAME, key);
        }

        public void Write(string key, string output)
        {
            CreateBucketIfDoesNotExist(BUCKET_NAME);
            WriteToS3Bucket(BUCKET_NAME, key, output);
        }

        public async Task WriteAsync(string key, string output)
        {
            CreateBucketIfDoesNotExist(BUCKET_NAME);
            await WriteToS3BucketAsync(BUCKET_NAME, key, output);
        }

        private string ReadFromS3Bucket(string bucketName, string keyName)
        {
            GetObjectRequest request = new GetObjectRequest();
            request.BucketName = bucketName;
            request.Key = keyName;
            string content;
            try
            {
                GetObjectResponse response = client.GetObject(request);
                StreamReader reader = new StreamReader(response.ResponseStream);
                content = reader.ReadToEnd();
            }
            catch (AmazonS3Exception)
            {
                // Key does not exist yet
                content = "";
            }
            return content;
        }

        private async Task<string> ReadFromS3BucketAsync(string bucketName, string keyName)
        {
            GetObjectRequest request = new GetObjectRequest();
            request.BucketName = bucketName;
            request.Key = keyName;
            string content;
            try
            {
                GetObjectResponse response = await client.GetObjectAsync(request);
                StreamReader reader = new StreamReader(response.ResponseStream);
                content = reader.ReadToEnd();
            }
            catch (AmazonS3Exception)
            {
                // Key does not exist yet
                content = "";
            }
            return content;
        }

        private void WriteToS3Bucket(string bucketName, string keyName, string content)
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = bucketName;
            request.Key = keyName;
            request.ContentBody = content;
            client.PutObject(request);
        }

        private async Task WriteToS3BucketAsync(string bucketName, string keyName, string content)
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = bucketName;
            request.Key = keyName;
            request.ContentBody = content;
            await client.PutObjectAsync(request);
        }

        private void CreateBucketIfDoesNotExist(string bucketName)
        {
            ListBucketsResponse response = client.ListBuckets();
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
                client.PutBucket(bucketName);
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
