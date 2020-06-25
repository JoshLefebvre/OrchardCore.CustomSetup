using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.CustomSetup.Events;

namespace OrchardCore.CustomSetup
{
    public class Startup : StartupBase
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            //Migrations
            //services.AddScoped<IDataMigration, CustomSetupMigrations>();

            services.AddScoped<ICustomTenantSetupEventHandler, AddAdminUserWhenTenantCreatedEventHandler>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
           routes.MapAreaControllerRoute
            (
                name: "CustomSetup",
                areaName: "OrchardCore.CustomSetup",
                pattern: "",//Overrides default Setup pattern
                defaults: new { controller = "CustomSetup", action = "Index" }
            );
        }
    }
}
