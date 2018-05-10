using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace WebAPIGetway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Action<IdentityServerAuthenticationOptions> options = o =>
            {
                //IdentityService认证服务的地址
                o.Authority = "http://localhost:9500";
                //IdentityService项目中Config类中定义的ApiName
                o.ApiName = "s2api"; //
                o.RequireHttpsMetadata = false;
                o.SupportedTokens = SupportedTokens.Both;
                //IdentityService项目中Config类中定义的Secret
                o.ApiSecret = "secret";
            };

            services.AddOcelot()
                .AddAdministration("/admin", options);
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOcelot().Wait();
        }
    }
}
