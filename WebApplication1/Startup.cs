using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using WebApplication1.Data;
using Csla;
using Csla.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Csla.Blazor.Client.Authentication;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Net.Security;
using System.Net;
using System.Security.Authentication;

namespace WebApplication1
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
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });



            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<CircuitHandler, TrackingCircuitHandler>();
            services.AddCsla();
            services.AddTransient(typeof(IDataPortal<>), typeof(DataPortal<>));
            services.AddTransient(typeof(Csla.Blazor.ViewModel<>), typeof(Csla.Blazor.ViewModel<>));
        services.AddAuthorizationCore(config =>
            {
                config.AddPolicy("IsAuthenticated",
                policy => policy.RequireAuthenticatedUser());
                config.AddPolicy("IsAdmin", policy => policy.RequireRole("admin", "supervisor", "manager"));
                config.AddPolicy("Thailand",
                policy => policy.RequireClaim(ClaimTypes.Country, "es"));
            });
            // services.AddBaseAddressHttpClient();
             services.AddScoped<HttpClient>(s =>
            {
               // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.

               return new HttpClient {
                   BaseAddress = new Uri("http://localhost:5000/api/DataPortal/")
               };
            });

            HttpClientHandler clientHandler = new HttpClientHandler {
                  AllowAutoRedirect = false,
                    ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
                    SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12
            };
            // services.AddHttpClient()
            //     .ConfigurePrimaryHttpMessageHandler(sp => new HttpClientHandler
            //     {
            //         AllowAutoRedirect = false,
            //         ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
            //         SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12
            //     });
            // clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
             HttpClient client = new HttpClient(clientHandler);
            services.AddSingleton<AuthenticationStateProvider, CslaAuthenticationStateProvider>();
            services.AddSingleton<CslaUserService>();
             
                services.AddHttpClient("extendedhandlerlifetime")
        .SetHandlerLifetime(TimeSpan.FromMinutes(5));
            // services.AddHttpClient()
            //     .ConfigurePrimaryHttpMessageHandler(sp => new HttpClientHandler
            //     {
            //         AllowAutoRedirect = false,
            //         ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
            //         SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12
            //     });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ServicePointManager.ServerCertificateValidationCallback +=
            // (sender, certificate, chain, errors) => {
            //     return true;
            // };
         
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
               
            });
            
      CslaConfiguration.Configure().
        ContextManager(typeof(Csla.Blazor.ApplicationContextManager)).
        DataPortal().
          DefaultProxy((typeof(Csla.DataPortalClient.HttpProxy)), "http://localhost:5000/api/DataPortal/");
    
        }
    }
}
