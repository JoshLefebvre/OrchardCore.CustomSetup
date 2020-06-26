# OrchardCore.CustomSetup
An Custom Orchard Core module that extends the existing setup module


## Goal:
Create new Tenants and automate emailing the tenant admin a link for `Tenant Setup` through workflows.


## Reason
My tenant creation flow needs to be very customizable and automated for my business. `Tenant Admins` will be responsible for administratrating their sites and I never want to give them `Super User` privileges as this is dangerous. This custom setup module will allow me to create the new tenant admin with a specific role and use a default account for the SuperUser that only I will have access to.


## Setting up your dev environment
1. **Prerequisites:** Make sure you have an up-to-date clone of [the Orchard Core repository](https://github.com/OrchardCMS/OrchardCore) on the `dev` branch. Please consult [the Orchard Core documentation](https://orchardcore.readthedocs.io/en/latest/) and make sure you have a working Orchard before you proceed. You'll also, of course, need all of Orchard Core's prerequisites for development (.NET Core, a code editor, etc.). The following steps assume some basic understanding of Orchard Core.
2. Clone the module under `[your Orchard Core clone's root]/src/OrchardCore.Modules`.
3. Add the existing project to the solution under `src/OrchardCore.Modules` in the solution explorer if you're using Visual Studio.
4. Add a reference to the module from the `OrchardCore.Cms.Web` project.
5. Add the module as a setup feature in the `OrchardCore.Cms.Web` as follows:
```csharp
services.AddOrchardCms()
    .AddSetupFeatures("OrchardCore.CustomSetup")
```
6. Comment out the `Setup` route in OrchardCore.Setup Startup.cs so that the new route will be used by default: 
```csharp
//routes.MapAreaControllerRoute(
//    name: "Setup",
//    areaName: "OrchardCore.Setup",
//    pattern: "",
//    defaults: new { controller = "Setup", action = "Index" }
//);
```
7. Build, run.