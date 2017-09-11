using System.Net;
using System.Net.Http;

namespace HelloPhoto
{
	class PhotoRepository
	{
		public HttpStatusCode Save(string filePath)
		{
			var client = new HttpClient();
			var stringContent = new StringContent(filePath);

			var result =
				client.PostAsync("http://hellophotoapi-prod.us-east-1.elasticbeanstalk.com/api/Photos", stringContent).Result;

			return result.StatusCode;
		}
	}
}
