using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer;
using Microsoft.Extensions.Logging;

namespace IdentityServerPractice
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            ///This Code adds identity Server
            services.AddIdentityServer()
                    //Creates temporary key material at startup time. This is for dev only scenarios when you don’t have a certificate to use. The generated key will be persisted to the file system so it stays stable between server restarts (can be disabled by passing false). This addresses issues when the client/api metadata caches get out of sync during development
                    .AddDeveloperSigningCredential()
                    //Add Test Users
                    .AddTestUsers(Config.GetUsers())
                    // Adds in Memory Clients
                    .AddInMemoryClients(Config.GetClients())
                    // Add In Memroy Identity Resources
                    .AddInMemoryIdentityResources(Config.GetIdentityResources());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory logger)
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
