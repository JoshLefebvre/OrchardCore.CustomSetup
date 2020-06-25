using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using LefeWareLearning.CustomSetup.Events;

namespace LefeWareLearning.Tenants
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
                areaName: "LefeWareLearning.CustomSetup",
                pattern: "",//Overrides default Setup pattern
                defaults: new { controller = "CustomTenantSetup", action = "Setup" }
            );
        }
    }
}
