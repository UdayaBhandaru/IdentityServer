using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MvcClient.HttpClientService
{

    public class HttpClientProvider:IHttpClientProvider
    {
        
        public HttpClientProvider(IHttpContextAccessor httpContextAccessor, IOptions<OpenIdConnectOptions> optionsAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _openIdConnectionOptions = optionsAccessor.Value;
        }
        private OpenIdConnectOptions _openIdConnectionOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _httpClient = new HttpClient();

        public async Task<HttpClient> GetClient(string BaseAddress)
        {
            _httpClient = new HttpClient();
            string accessToken = string.Empty;
            var currentContext = _httpContextAccessor.HttpContext;
            var expires_at = await currentContext.GetTokenAsync("expires_at");
            if (string.IsNullOrWhiteSpace(expires_at)
                || ((DateTime.Parse(expires_at).AddSeconds(-60)).ToUniversalTime()
                < DateTime.UtcNow))
            {
                accessToken = await RenewTokens();
            }
            else
            {
                // get access token
                accessToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }


            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                _httpClient.SetBearerToken(accessToken);
            }

            _httpClient.BaseAddress = new Uri(BaseAddress);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }

        private async Task<string> RenewTokens()
        {
            // get the current HttpContext to access the tokens
            var currentContext = _httpContextAccessor.HttpContext;

            // get the metadata
            var discoveryClient = new DiscoveryClient(_openIdConnectionOptions.Authority);
            var metaDataResponse = await discoveryClient.GetAsync();

            // create a new token client to get new tokens
            var tokenClient = new TokenClient(metaDataResponse.TokenEndpoint, _openIdConnectionOptions.ClientId, _openIdConnectionOptions.ClientSecret);

            // get the saved refresh token
            var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            // refresh the tokens
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

            if (!tokenResult.IsError)
            {
                // Save the tokens. 

                // get auth info
                var authenticateInfo = await currentContext.AuthenticateAsync("cookies");

                // create a new value for expires_at, and save it
                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                authenticateInfo.Properties.UpdateTokenValue("expires_at",expiresAt.ToString("o", CultureInfo.InvariantCulture));

                authenticateInfo.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken,tokenResult.AccessToken);
                authenticateInfo.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken,tokenResult.RefreshToken);

                // we're signing in again with the new values.  
                await currentContext.SignInAsync("cookies", authenticateInfo.Principal, authenticateInfo.Properties);

                // return the new access token 
                return tokenResult.AccessToken;
            }
            else
            {
                throw new Exception("Problem encountered while refreshing tokens.",
                    tokenResult.Exception);
            }
        }

        public async Task<TokenRevocationClient> GetTokenRevocationClient()
        {
            var discoverClient = new DiscoveryClient(_openIdConnectionOptions.Authority);
            var metaDataResponmse = await discoverClient.GetAsync();
            var tokenrevocationClient = new TokenRevocationClient(metaDataResponmse.RevocationEndpoint, _openIdConnectionOptions.ClientId, _openIdConnectionOptions.ClientSecret);
            return tokenrevocationClient;

        }
    }
}
