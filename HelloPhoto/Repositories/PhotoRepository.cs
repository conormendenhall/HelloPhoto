using HelloPhoto.Models;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Diagnostics;
using Amazon;
using Amazon.Runtime;
using HelloPhoto.Repositories;

namespace HelloPhoto
{
	class PhotoRepository
	{
		public async void Save(Photo photo)
		{
		    AmazonS3Config S3Config = new AmazonS3Config()
		    {
                UseHttp = true,
                RegionEndpoint = RegionEndpoint.USEast2
		    };

            IAmazonS3 client;
            using (client = new AmazonS3Client("AKIAIPKY64SW5RVZ6MUA", "NV2PBsukXLMns64JJIvf+0kA8O+8jk7RCpUb/KsS", S3Config))
            {
                Debug.WriteLine("Uploading an object");
                try
                {
                    PutObjectRequest request = new PutObjectRequest()
                    {
                        BucketName = "hellophoto-det",
                        Key = photo.PhotoId + ".png",
                        FilePath = photo.FilePath,
                        ContentType = "image/png"
                    };
                    request.Metadata.Add("x-amz-meta-title", "someTitle");
                    PutObjectResponse response1 = await client.PutObjectAsync(request);

                    var test = new TwitterRepository();
                    test.TweetItAsync(photo.PhotoId + ".png");
                }
                catch (AmazonServiceException amazonS3Exception)
                {
                    if (amazonS3Exception.ErrorCode != null &&
                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                        ||
                        amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                    {
                        Debug.WriteLine("Check the provided AWS Credentials.");
                        Debug.WriteLine(
                            "For service sign up go to http://aws.amazon.com/s3");
                    }
                    else
                    {
                        Debug.WriteLine(
                            "Error occurred. Message:'{0}' when writing an object"
                            , amazonS3Exception.Message);
                    }
                }
            }
		}
	}
}
