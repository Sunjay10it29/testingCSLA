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
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication;
using WebApplication1.Data;
using Csla;
using Csla.Configuration;
using System.Security.Claims;
using System.Net;
using System.Net.Http;
using Csla.DataPortalClient;
using System.Text;
using Microsoft.AspNetCore.Blazor.Hosting;


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
            services.AddSingleton<AuthenticationStateProvider, CustomAuthStateProvider>();
            services.AddSingleton<CurrentUserService>();
            services.AddCsla();
            if (!services.Any(x => x.ServiceType == typeof(HttpClient)))
{
	// Setup HttpClient for server side in a client side compatible fashion
            services.AddScoped<HttpClient>(s =>
                {
                    // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
                            var uriHelper = s.GetRequiredService<NavigationManager>();
                        return new HttpClient
                    {
                        BaseAddress = new Uri(uriHelper.BaseUri)
                    };
                });
            }
            // services.AddTransient(typeof(IDataPortal<>), typeof(DataPortal<>));
            // services.AddTransient(typeof(Csla.Blazor.ViewModel<>), typeof(Csla.Blazor.ViewModel<>));
            services.AddAuthorizationCore(config =>
                {
                    config.AddPolicy("IsAuthenticated",
                    policy => policy.RequireAuthenticatedUser());
                    config.AddPolicy("IsAdmin", policy => policy.RequireRole("admin", "supervisor", "manager"));
                    config.AddPolicy("Thailand",
                    policy => policy.RequireClaim(ClaimTypes.Country, "es"));
                });
            // services.AddBaseAddressHttpClient();
           

            // Pass the handler to httpclient(from you are calling api)
            // HttpClient client = new HttpClient(clientHandler);
            ServicePointManager.ServerCertificateValidationCallback += 
                (sender, cert, chain, sslPolicyErrors) => true;
           

            // services.AddHttpClient("extendedhandlerlifetime")
            // .SetHandlerLifetime(TimeSpan.FromMinutes(5));
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
            // app.UseCsla(c =>
            // {
            //     c.DataPortal()
            //     .DefaultProxy(typeof(CustomDataPortalProxyFactory), "http://localhost:5000/api/dataportaltext/");
            // });

        }

    }
}
public class CustomDataPortalProxyFactory : IDataPortalProxyFactory
{
    private static System.Type _proxyType;


    public Csla.DataPortalClient.IDataPortalProxy Create(System.Type objectType)
    {

        if (_proxyType == null)
        {
            string proxyTypeName = Csla.ApplicationContext.DataPortalProxy;
            if (proxyTypeName == "Local")
                _proxyType = typeof(LocalProxy);
            else
                _proxyType = System.Type.GetType(proxyTypeName, true, true);
        }

        return (Csla.DataPortalClient.IDataPortalProxy)Csla.Reflection.MethodCaller.CreateInstance(_proxyType);
    }

    public void ResetProxyType()
    {
        throw new System.NotImplementedException();
    }
}
