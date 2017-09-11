using System;
using System.Net;
using System.Net.Http;

namespace HelloPhoto
{
	interface IPhotoRepository
	{
		HttpStatusCode Save(string filePath);
	}

	class PhotoRepository : IPhotoRepository
	{
		public HttpStatusCode Save(string filePath)
		{
			HttpClient client = new HttpClient();
			var stringContent = new StringContent(filePath);

			var result =
				client.PostAsync("http://hellophotoapi-prod.us-east-1.elasticbeanstalk.com/api/Photos", stringContent).Result;

			return result.StatusCode;
		}
	}
}
