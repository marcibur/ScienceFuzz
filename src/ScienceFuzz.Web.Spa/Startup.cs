using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using ScienceFuzz.Web.Spa.Configuration;
using System.Net.Http;

namespace ScienceFuzz.Web.Spa
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Settings>();
            services.AddTransient<HttpClient>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
