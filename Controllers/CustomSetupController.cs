using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.CustomSetup.Events;
using OrchardCore.CustomSetup.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Email;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules;
using OrchardCore.Setup.Services;

namespace OrchardCore.CustomSetup.Controllers
{
    public class CustomSetupController : Controller
    {
        private readonly IShellHost _shellHost;
        private readonly ISetupService _setupService;
        private readonly ShellSettings _shellSettings;
        private readonly IClock _clock;
        private readonly ILogger _logger;
        private readonly IStringLocalizer S;
        private readonly IEmailAddressValidator _emailAddressValidator;
        private readonly IConfiguration _configuration;

        public CustomSetupController(
            IShellHost shellHost,
            ISetupService setupService,
            ShellSettings shellSettings,
            IClock clock,
            ILogger<CustomSetupController> logger,
            IStringLocalizer<CustomSetupController> localizer,
            IEmailAddressValidator emailAddressValidator,
            IConfiguration configuration)
        {
            _shellHost = shellHost;
            _setupService = setupService;
            _shellSettings = shellSettings;
            _clock = clock;
            _logger = logger;
            S = localizer;
            _emailAddressValidator = emailAddressValidator;
            _configuration = configuration;
        }

        public async Task<ActionResult> Index(string token)
        {
            if (!await IsValidRequest(token))
            {
                return BadRequest(S["Error with tenant setup link. Please contact support to issue a new link"]);
            }

            return View(new CustomSetupViewModel() { Secret = token});
        }


        [HttpPost, ActionName("Index")]
        public async Task<ActionResult> IndexPOST(CustomSetupViewModel model)
        {
            if (!await IsValidRequest(model.Secret))
            {
                return BadRequest(S["Error with tenant setup link. Please contact support to issue a new link"]);
            }
            if(!IsModelValid(model))
            {
                return View(model);
            }

            var setupContext = await CreateSetupContext(model.SiteName, model.SiteTimeZone);
            var executionId = await _setupService.SetupAsync(setupContext);

            // Check if a component in the Setup failed
            if (setupContext.Errors.Any())
            {
                foreach (var error in setupContext.Errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                return View(model);
            }

            var shellScope = await _shellHost.GetScopeAsync(_shellSettings);
            await shellScope.UsingAsync(async scope =>
            {
                void reportError(string key, string message)
                {
                    setupContext.Errors[key] = message;
                }

                // Invoke modules to react to the setup event
                var customsetupEventHandlers = scope.ServiceProvider.GetServices<ICustomTenantSetupEventHandler>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomSetupController>>();
                await customsetupEventHandlers.InvokeAsync(x => x.Setup(model.Email, model.Password, reportError), logger);
            });

            return Redirect($"~/portal");
        }

        private async Task<SetupContext> CreateSetupContext(string siteName, string timeZone)
        {
            var recipes = await _setupService.GetSetupRecipesAsync();
            var selectedRecipe = recipes.FirstOrDefault(x => x.Name == _shellSettings["RecipeName"]);

            var setupContext = new SetupContext
            {
                ShellSettings = _shellSettings,
                SiteName = siteName,
                EnabledFeatures = null, // default list,
                AdminUsername = _configuration["AdminEmail"],
                AdminEmail = _configuration["AdminEmail"],
                AdminPassword = _configuration["AdminPassword"],
                Errors = new Dictionary<string, string>(),
                Recipe = selectedRecipe,
                SiteTimeZone = timeZone,
                DatabaseProvider = _shellSettings["DatabaseProvider"],
                DatabaseConnectionString = _shellSettings["ConnectionString"],
                DatabaseTablePrefix = _shellSettings["TablePrefix"]
            };

            return setupContext;
        }

        private async Task<bool> IsValidRequest(string token)
        {
            if (_shellSettings.State != OrchardCore.Environment.Shell.Models.TenantState.Uninitialized)
            {
                _logger.LogWarning("An attempt to setup a tenant that was already running was made", _shellSettings.Name);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(_shellSettings["Secret"]))
            {
                if (string.IsNullOrEmpty(token) || !await IsTokenValid(token))
                {
                    _logger.LogWarning("An attempt to access '{TenantName}' without providing a secret was made", _shellSettings.Name);
                    return false;
                }
            }

            return true;
        }


        private bool IsModelValid(CustomSetupViewModel model)
        {
            if (String.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), S["The password is required."]);
            }

            if (model.Password != model.PasswordConfirmation)
            {
                ModelState.AddModelError(nameof(model.PasswordConfirmation), S["The password confirmation doesn't match the password."]);
            }

            if (!_emailAddressValidator.Validate(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), S["Invalid email."]);
            }

            if (!ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(_shellSettings["Description"]))
                {
                    model.Description = _shellSettings["Description"];
                }
                return false;
            }
            return true;
        }

        private async Task<bool> IsTokenValid(string token)
        {
            try
            {
                var result = false;

                var shellScope = await _shellHost.GetScopeAsync(ShellHelper.DefaultShellName);

                await shellScope.UsingAsync(scope =>
                {
                    var dataProtectionProvider = scope.ServiceProvider.GetRequiredService<IDataProtectionProvider>();
                    var dataProtector = dataProtectionProvider.CreateProtector("Tokens").ToTimeLimitedDataProtector();

                    var tokenValue = dataProtector.Unprotect(token, out var expiration);

                    if (_clock.UtcNow < expiration.ToUniversalTime())
                    {
                        if (_shellSettings["Secret"] == tokenValue)
                        {
                            result = true;
                        }
                    }

                    return Task.CompletedTask;
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in decrypting the token");
            }

            return false;
        }
    }
}
