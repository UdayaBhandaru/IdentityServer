using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using MvcClient.Authorization;
using Microsoft.AspNetCore.Http;
using MvcClient.HttpClientService;

namespace MvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            var config = builder.Build();
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(options =>
                                        {
                                            options.DefaultScheme ="cookies";
                                            options.DefaultChallengeScheme ="oidc";
                                        })
                                        .AddCookie("cookies")
                                        .AddOpenIdConnect("oidc", options => { options.GetClaimsFromUserInfoEndpoint = true; GetOpenIdConnectOptions(options); });
            services.AddAuthorization(authorizationOptions => authorizationOptions.AddPolicy("IsContributor",
            policybuilder => 
            {
                policybuilder.RequireAuthenticatedUser();
                policybuilder.AddRequirements(new AdministratorRequirement());
            }));

            services.AddSingleton<IAuthorizationHandler, IsContributor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IHttpClientProvider, HttpClientProvider>();
            services.Configure<OpenIdConnectOptions>( options => { Configuration.GetSection("openIdConnectOptions").Bind(options); options.SignInScheme = "cookies"; });
            
            services.Configure<ApiClientOption>(options => Configuration.GetSection("ApiClientOption").Bind(options));
            services.AddMvc();
        }

        private void GetOpenIdConnectOptions(OpenIdConnectOptions options)
        {
            
            Configuration.GetSection("openIdConnectOptions").Bind(options);
            options.Events.OnUserInformationReceived = userinformationRecievedContext =>
            {
                
                return Task.FromResult(0);
            };

            options.Events.OnTokenValidated = tokenValidatedContext =>
            {

                return Task.FromResult(0);
            };
            options.SignInScheme = "cookies";
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //Maps Claims to WS Stahdards
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
