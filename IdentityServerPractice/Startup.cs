using IdentityServer.Data;
using IdentityServer.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            ///This Code adds identity Server
            services.AddIdentityServer()
                    //Creates temporary key material at startup time. This is for dev only scenarios when you don’t have a certificate to use. The generated key will be persisted to the file system so it stays stable between server restarts (can be disabled by passing false). This addresses issues when the client/api metadata caches get out of sync during development
                    .AddDeveloperSigningCredential()
                    //Add Test Users
                    //.AddTestUsers(Config.GetUsers())
                    .AddAspNetIdentity<ApplicationUser>()
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
