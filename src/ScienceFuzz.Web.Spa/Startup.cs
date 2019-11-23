using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using ScienceFuzz.Http.Client;
using ScienceFuzz.Web.Spa.Configuration;
using System.Net.Http;

namespace ScienceFuzz.Web.Spa
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Settings>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<ScienceFuzzClient>(x =>
            {
                var http = x.GetRequiredService<HttpClient>();
                var settings = x.GetRequiredService<Settings>();
                var apiUriBase = settings.ApiRootUri;

                return new ScienceFuzzClient(http, apiUriBase);
            });
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
