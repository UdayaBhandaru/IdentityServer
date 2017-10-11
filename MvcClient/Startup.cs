using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

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
                                            options.DefaultScheme = Configuration.GetValue<string>("DefaultScheme");
                                            options.DefaultChallengeScheme = Configuration.GetValue<string>("DefaultChallengeScheme");
                                        })
                                        .AddCookie(Configuration.GetValue<string>("DefaultScheme"))
                                        .AddOpenIdConnect(Configuration.GetValue<string>("DefaultChallengeScheme"), options=> GetOpenIdConnectOptions(options));
           services.Configure<OpenIdConnectOptions>( options => Configuration.GetSection("openIdConnectOptions").Bind(options));

            services.AddMvc();
        }

        private void GetOpenIdConnectOptions(OpenIdConnectOptions options)
        {
            //options =>
            //{
            //    options.Authority = "https://localhost:44314/";
            //    options.RequireHttpsMetadata = true;
            //    options.ClientId = "MVCClientId";
            //    options.ClientSecret = "secret";
            //    options.ResponseType = "code id_token";
            //    options.SignInScheme = "MyCookieAuthenticationScheme";
            //    options.SaveTokens = true;

            //    options.GetClaimsFromUserInfoEndpoint = true;
            //}
            Configuration.GetSection("openIdConnectOptions").Bind(options);
            options.Events.OnUserInformationReceived = userinformationRecievedContext =>
            {
                return Task.FromResult(0);
            };
            options.Events.OnTokenValidated = tokenValidationContext =>
            {
                return Task.FromResult(0);
            };
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
