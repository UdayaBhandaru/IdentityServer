﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                                        {
                                            options.DefaultScheme = "MyCookieAuthenticationScheme";
                                            options.DefaultChallengeScheme = "oidc";
                                        })
                                        .AddCookie("MyCookieAuthenticationScheme")
                                        .AddOpenIdConnect("oidc", options =>
                                        {
                                            options.Authority = "https://localhost:44314/";
                                            options.RequireHttpsMetadata = true;
                                            options.ClientId = "MVCClientId";
                                            options.ClientSecret = "secret";
                                            options.ResponseType = "code id_token";
                                            options.SignInScheme = "MyCookieAuthenticationScheme";
                                        });

            services.AddMvc();
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
