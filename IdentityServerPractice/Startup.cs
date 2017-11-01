using System;
using System.Collections.Generic;
using IdentityServer.Data;
using IdentityServer.Data.Models;
using IdentityServer.Services;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using System.Threading.Tasks;
using System.Linq;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddMvc();

            services.AddTransient<IEmailSender, EmailSender>();
            ///This Code adds identity Server
            services.AddIdentityServer()
                    //Creates temporary key material at startup time. This is for dev only scenarios when you don’t have a certificate to use. The generated key will be persisted to the file system so it stays stable between server restarts (can be disabled by passing false). This addresses issues when the client/api metadata caches get out of sync during development
                    //.AddDeveloperSigningCredential()
                    .AddSigningCredential(new X509Certificate2(Configuration.GetSection("SignInCertificatePath").Value, Configuration.GetSection("SignInCertificatePassword").Value))
                    //Add Test Users
                    //.AddTestUsers(Config.GetUsers())
                    .AddAspNetIdentity<ApplicationUser>()
                    // Adds in Memory Clients
                    .AddInMemoryClients(ConfigureClientResources())

                    //Add Custom profile Service
                    .AddProfileService<AspNetIdentityProfileService>()
                    // Add In Memroy API Resources
                    .AddInMemoryApiResources(ConfigureApiResources())
                    .AddEndpoint<ServiceDocumentEndPoint>("serviceDoc", "/servicedoc/data")
                    

                    // Add In Memroy Identity Resources
                    .AddInMemoryIdentityResources(Config.GetIdentityResources());


            



        }

        private IEnumerable<ApiResource> ConfigureApiResources()
        {
            var apiresourcesfromConfig = new List<ApiResource>();
            var apiResources = new List<ApiResource>();
            Configuration.GetSection("Clients:APiClient:ApiResource").Bind(apiresourcesfromConfig);
            apiresourcesfromConfig.ForEach(apiResource => apiResources.Add(
             new ApiResource(apiResource.Name, apiResource.UserClaims) { ApiSecrets=apiResource.ApiSecrets}));
            apiResources.ForEach(clientResource =>
            {
                var clientSecrets = new List<Secret>();

                foreach (var clientSecret in clientResource.ApiSecrets)

                {
                    clientSecrets.Add(new Secret(clientSecret.Value.Sha256(), null));
                }
                clientResource.ApiSecrets = clientSecrets;
            });
            return apiResources;
        }
        private IEnumerable<Client> ConfigureClientResources()
        {
            var clientResources = new List<Client>();
            Configuration.GetSection("Clients:WebClient:Client").Bind(clientResources);

            clientResources.ForEach(clientResource =>
            {
                var clientSecrets = new List<Secret>();

                foreach (var clientSecret in clientResource.ClientSecrets)

                {
                    clientSecrets.Add(new Secret(clientSecret.Value.Sha256(), null));
                }
                clientResource.ClientSecrets = clientSecrets;
            });
            return clientResources;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
        {
            logger.AddConsole();
            logger.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseIdentityServer();
            
            
            app.UseMvc();
            app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();
        }
    }
}
