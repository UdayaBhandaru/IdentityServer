using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "Frank",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Frank"),
                        new Claim("family_name", "Underwood"),
                        new Claim("address", "1, Main Road"),
                        new Claim("role", "FreeUser")
                    }
                },
                new TestUser
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "Claire",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Claire"),
                        new Claim("family_name", "Underwood"),
                        new Claim("address", "2, Big Street"),
                        new Claim("role", "PayingUser")
                    }
                }
            };
        
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {

                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }
        public static IEnumerable<ApiResource> GetApiResources()
        {
            var apiResources = new List<ApiResource>
            {
                new ApiResource("apiClient",new List<string>() { "role" })
            };
            return apiResources;
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>() {
                
                new Client()
                {
                    ClientName="MVCClient",
                    ClientId="MVCClientId",
                    ClientSecrets={ new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.Hybrid,
                    AllowOfflineAccess = true,
                    IdentityTokenLifetime=5,
                    AccessTokenLifetime=120,
                    UpdateAccessTokenClaimsOnRefresh=true,
                    RedirectUris =new List<string>
                    {
                        "https://localhost:44387/signin-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "apiClient"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44387/signout-callback-oidc"
                    }, 
                    RequireConsent=false,
                    AlwaysIncludeUserClaimsInIdToken=true
                }

            };
        }
    }
}