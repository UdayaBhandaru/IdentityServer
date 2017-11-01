using IdentityServer4.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IdentityServer
{
    public class ServiceDocumentEndPoint : IEndpointHandler
    {
        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            return await ProcessServiceDocumentAsync(context);
        }

        private async Task<IEndpointResult> ProcessServiceDocumentAsync(HttpContext context)
        {
            return new ServiceDocumentEndPointResult();
        }
    }

    public class ServiceDocumentEndPointResult : IEndpointResult
    {
        public async Task ExecuteAsync(HttpContext context)
        {
          await  context.Response.WriteJsonAsync("Test Content");
        }
    }
}
