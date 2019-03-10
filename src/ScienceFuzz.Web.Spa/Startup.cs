using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using ScienceFuzz.Web.Spa.Components;

namespace ScienceFuzz.Web.Spa
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddSingleton<State>();

        public void Configure(IComponentsApplicationBuilder app) =>
            app.AddComponent<App>("app");
    }
}
