using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using HelloPhoto.Models;
using HttpClient = Windows.Web.Http.HttpClient;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace HelloPhoto.Repositories
{
    public class TwitterRepository
    {
        public async void TweetItAsync(string id)
        {
            HttpBaseProtocolFilter aHBPF = new HttpBaseProtocolFilter();
            aHBPF.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
            aHBPF.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
            aHBPF.IgnorableServerCertificateErrors.Add(ChainValidationResult.IncompleteChain);

            var client = new HttpClient(aHBPF);

            var x = new {
                image = id,
                hashtag = (AdminSettings.Event?.TwitterHashTag ?? "FunWithIoT")};
            
            var stringContent = new HttpStringContent("\"{\'image\':\'"+id+"\',\'hashtag\':\'"+ (AdminSettings.Event?.TwitterHashTag ?? "FunWithIoT") + "\'}\"", UnicodeEncoding.Utf8, "application/json");
            var result =
                await client.PostAsync(new Uri("https://8kasriow2b.execute-api.us-east-2.amazonaws.com/Prod/twitter"), stringContent);
            
        }
    }
}
