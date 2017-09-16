using HelloPhoto.Models;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace HelloPhoto
{
	class PhotoRepository
	{
		public async Task<Photo> Save(Photo photo)
		{
            //var client = new HttpClient();
            //var stringContent = new StringContent(JsonConvert.SerializeObject(photo), Encoding.UTF8, "application/json");

            //var result =
            //	await client.PostAsync("http://hellophotoapi-prod.us-east-1.elasticbeanstalk.com/api/Photos", stringContent);

            //if (result.StatusCode == HttpStatusCode.OK)
            //{
            //	return photo;
            //}

            IAmazonS3 client;
            client = new AmazonS3Client(Amazon.RegionEndpoint.USEast2);
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = "hellophoto-det",
                Key = photo.PhotoId.ToString(),
                FilePath = photo.FilePath
            };
            PutObjectResponse response2 = await client.PutObjectAsync(request);

            return null;
		}
	}
}
