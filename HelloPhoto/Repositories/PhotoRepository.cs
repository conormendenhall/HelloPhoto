using HelloPhoto.Models;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Net.Http;
using Amazon;

namespace HelloPhoto
{
    class PhotoRepository
    {
        private const string accessKey = "zzz";
        private const string secret = "zzz";

        public async void Save(Photo photo)
        {
            var client = new HttpClient();
            {
                var friendlyFilename = Path.GetFileName(photo.FilePath);
                client.Timeout = TimeSpan.FromSeconds(25);

                client.BaseAddress = new Uri("http://hellophotoapi-prod.us-east-1.elasticbeanstalk.com/");

                var fileData = new ByteArrayContent(File.ReadAllBytes(photo.FilePath));
                
                var multiContent = new MultipartFormDataContent();
                multiContent.Add(fileData, "file", friendlyFilename);

                //this filename needs to match the name specified in the agreement.
                //for awareness, name=file1 is just a field name like from a form. Might not need this, 
                //but mvc core api requires the name field to see the file
                multiContent.Headers.Add("Content-Disposition", "form-data; file=\"" + friendlyFilename + "\"");


                //red rover, red rover, send the data over
                var resultPre = client.PostAsync("api/photo/", multiContent);

                var result = resultPre.Result;

                Console.WriteLine($"{(int)result.StatusCode} - {result.StatusCode.ToString()}");
                Console.WriteLine("sxm result message: " + result.Content.ReadAsStringAsync().Result);
            }
        }


        public void RegisterFace(Models.FaceOff photo)
        {
            var client = new HttpClient();
            {
                var friendlyFilename = Path.GetFileName(photo.FilePath);
                client.Timeout = TimeSpan.FromSeconds(35);

                client.BaseAddress = new Uri("http://hellophotoapi-prod.us-east-1.elasticbeanstalk.com/");

                var fileData = new ByteArrayContent(File.ReadAllBytes(photo.FilePath));

                var multiContent = new MultipartFormDataContent
                {
                    {new StringContent(photo.FirstName), "FirstName" },
                    {new StringContent(photo.LastName), "LastName" },
                    {new StringContent(photo.Email), "Email" },
                    {new StringContent("TSFaces"), "CollectionName" },
                    {fileData, "file", friendlyFilename }
                };

                //this filename needs to match the name specified in the agreement.
                //for awareness, name=file1 is just a field name like from a form. Might not need this, 
                //but mvc core api requires the name field to see the file
                multiContent.Headers.Add("Content-Disposition", "form-data; file=\"" + friendlyFilename + "\"");
                    
                //red rover, red rover, send the data over
                var resultPre = client.PostAsync("api/facewatch/", multiContent);

                var result = resultPre.Result;
            }
        }

        public async Task<byte[]> Get(string key)
        {
            AmazonS3Config S3Config = new AmazonS3Config()
            {
                UseHttp = true,
                RegionEndpoint = RegionEndpoint.USEast2
            };

            IAmazonS3 client;
            using (client = new AmazonS3Client(accessKey, secret, S3Config))
            {
                var req = new GetObjectRequest()
                {
                    BucketName = "hellophoto-det",
                    Key = key
                };

                var data = client.GetObjectAsync(req).Result;
                return ReadStream(data.ResponseStream);
            }
        }

        private static byte[] ReadStream(Stream responseStream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
