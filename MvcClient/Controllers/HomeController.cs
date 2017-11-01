using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using Microsoft.AspNetCore.Authorization;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using MvcClient.HttpClientService;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private ApiClientOption _apiClientOption;
        private IHttpClientProvider _httpClientProvider;
        public HomeController(IHttpClientProvider httpClientProvider,IOptions<ApiClientOption> apiClientOption)
        {
            _apiClientOption = apiClientOption.Value;
           _httpClientProvider = httpClientProvider;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> About()
        {
            await WriteOutIdentityInformation();

            var serviceDocEndpoint= await _httpClientProvider.GetServiceDocumentEndPoint();
           var sdcontent=await serviceDocEndpoint.GetStringAsync("");
            // get the saved identity token
            ViewData["IdentityToken"] = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            //get the saved access token

            ViewData["AccessToken"] = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var RefreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken); 
            var expires_at = await HttpContext.GetTokenAsync("expires_at");
            @ViewData["Expiration"] = DateTime.Parse(expires_at).AddSeconds(-60).ToUniversalTime();
            @ViewData["UTC"] = DateTime.UtcNow;

            //
            var httpClient = await _httpClientProvider.GetClient(_apiClientOption.ApiClientBaseAddress);
            var content = await httpClient.GetStringAsync("values\\Welcome");
            ViewData["Content"] = content;
            return View();
        }

        public async Task<IActionResult> Logout()
        {
           
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            if (!string.IsNullOrEmpty(accessToken))
            {
                var tokenrevocationClient = await _httpClientProvider.GetTokenRevocationClient();
                var refreshAccessTokenResponse = await tokenrevocationClient.RevokeAccessTokenAsync(accessToken);
                
            }
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var tokenrevocationClient = await _httpClientProvider.GetTokenRevocationClient();
                var refreshRefreshTokenResponse = await tokenrevocationClient.RevokeRefreshTokenAsync(refreshToken);
            }
            //var response = await userInfoEndPoint.GetAsync(accessToken);
            await this.HttpContext.SignOutAsync("cookies");
           await this.HttpContext.SignOutAsync("oidc");
            return View("Contact");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var httpClient = await _httpClientProvider.GetClient(_apiClientOption.ApiClientBaseAddress);
            var content = await httpClient.GetAsync("values\\Admin");

            if(content.IsSuccessStatusCode)
            {
                ViewBag.Content = "Authorized";
            }
            
            return View();
        }

        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> Admin()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var httpClient = await _httpClientProvider.GetClient(_apiClientOption.ApiClientBaseAddress);
            var content = await httpClient.GetStringAsync("values\\Admin");

            ViewBag.Content = content;
            return View("Contribute");
        }

        [Authorize(Policy = "IsContributor")]
        public async Task<IActionResult> Contribute()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var httpClient = await _httpClientProvider.GetClient(_apiClientOption.ApiClientBaseAddress);
            var content = await httpClient.GetStringAsync("values\\Contribute");

            ViewBag.Content = content;
            return View("Contribute");
        }

        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> Reader()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var httpClient = await _httpClientProvider.GetClient(_apiClientOption.ApiClientBaseAddress);
            var content = await httpClient.GetStringAsync("values\\Reader");

            ViewBag.Content = content;
            return View("Contribute");
        }


        public async Task WriteOutIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            //get the saved access token

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            
            // write Identity Token out
            Debug.WriteLine($"Identity token: {identityToken}");


            // write Access Token out
            Debug.WriteLine($"Access token: {accessToken}");
            // write out the user claims
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }
    }
}
