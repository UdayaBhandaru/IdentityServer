using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using IdentityModel;

namespace APIClient.Controllers
{
    [Route("[controller]")]
    
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]

        [Authorize(Roles = "Administrator")]
        [Route("Admin")]
        public string Admin()
        {
            return "You are the Admin Welcome Home.";
        }

        [Authorize(Policy ="IsContributor")]
        [Route("Contribute")]
        public string Contribute()
        {
            return "You are the Contributor. Welcome Home.";
        }

        [Authorize(Roles = "User")]
        [Route("Reader")]
        public string Reader()
        {
            return "You are the User. Welcome Home.";
        }


        [Authorize]
        [Route("Welcome")]
        public async Task<string> Welcome()
        {
            HttpClient _httpClient = new HttpClient();
           _httpClient.BaseAddress = new Uri("https://localhost:44325/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            var authorizationHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                var header = authorizationHeader.Trim();
                if (header.StartsWith(OidcConstants.AuthenticationSchemes.AuthorizationHeaderBearer))
                {
                    var value = header.Substring(OidcConstants.AuthenticationSchemes.AuthorizationHeaderBearer.Length).Trim();
                   // var accessToken = await this.HttpContext.GetTokenAsync("access_token");
                    _httpClient.SetBearerToken(value);
                    var content = await _httpClient.GetAsync("Values/Welcome");
                }
            }
                    
            return "Done";
        }
        
    }
}
