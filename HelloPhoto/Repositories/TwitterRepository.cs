using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HelloPhoto.Repositories
{
    public class TwitterRepository
    {
        public async void TweetItAsync(string id)
        {
            var client = new HttpClient();
            
            
            var stringContent = new StringContent("\"" + id + "\"", Encoding.UTF8, "application/json");
                     
            var result =
                await client.PostAsync("https://8kasriow2b.execute-api.us-east-2.amazonaws.com/Prod/twitter", stringContent);
            
        }
    }
}
