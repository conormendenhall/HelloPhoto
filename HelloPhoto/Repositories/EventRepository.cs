using System;
using System.Collections.Generic;
using System.Linq;
using HelloPhoto.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HelloPhoto.Repositories
{
	class EventRepository
	{
		public async Task<List<Event>> Get()
		{
		    try
		    {
		        var client = new HttpClient();

		        var result = client.GetAsync("http://hellophotoapi-prod.us-east-1.elasticbeanstalk.com/api/event").Result;

		        if (result.StatusCode == HttpStatusCode.OK)
		        {
		            var datas = await result.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<Event>>(datas);
		        }
            }
		    catch (Exception ex)
		    {
		        // ignored
		    }
            
		    return null;
		}
	}
}
