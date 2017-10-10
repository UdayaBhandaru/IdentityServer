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
namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> About()
        {
            await WriteOutIdentityInformation();
            ViewData["userid"] =await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.UserId);
            ViewData["userName"]  = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.Username);
            return View();
        }

        public IActionResult Logout()
        {
            this.HttpContext.SignOutAsync("MyCookieAuthenticationScheme");
            this.HttpContext.SignOutAsync("oidc");
            return View("Contact");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
