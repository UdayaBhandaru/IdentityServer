using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MvcClient.HttpClientService
{
    public interface IHttpClientProvider
    {
        Task<HttpClient> GetClient(string baseAddress);

        Task<TokenRevocationClient> GetTokenRevocationClient();


        Task<HttpClient> GetServiceDocumentEndPoint();
    }
}
