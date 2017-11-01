using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CustomAuthorizationRequirement;
using Microsoft.AspNetCore.Authorization;

namespace ApiCLient2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // https://localhost:44325/
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAuthentication("Bearer")
               .AddIdentityServerAuthentication(options => GetIdentityServerAuthenticationOptions(options));
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        private void GetIdentityServerAuthenticationOptions(IdentityServerAuthenticationOptions options)
        {

            Configuration.GetSection("IdentityServerAuthenticationOptions").Bind(options);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}

