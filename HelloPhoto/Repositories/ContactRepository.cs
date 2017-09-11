using HelloPhoto.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HelloPhoto.Repositories
{
	class ContactRepository
	{
		public async Task<string> Save(Contact contact)
		{
			var client = new HttpClient();

			var stringContent = new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8, "application/json");

			var result =
				await client.PostAsync("http://hellophotoapi-prod.us-east-1.elasticbeanstalk.com/api/Users", stringContent);

			if(result.StatusCode == HttpStatusCode.OK)
			{
				return contact.Id;
			}

			return null;
		}
	}
}
