using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using HttpClient = Windows.Web.Http.HttpClient;

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
            //aHBPF.IgnorableServerCertificateErrors.Add(ChainValidationResult.OtherErrors);
            //aHBPF.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidSignature);
            
            var client = new HttpClient(aHBPF);

            var stringContent = new HttpStringContent("\"" + id + "\"", Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                     
            var result =
                await client.PostAsync(new Uri("https://8kasriow2b.execute-api.us-east-2.amazonaws.com/Prod/twitter"), stringContent);
            
        }
    }
}
