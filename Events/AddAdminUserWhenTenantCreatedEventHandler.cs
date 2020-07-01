using System;
using System.Threading.Tasks;
using OrchardCore.CustomSetup.Events;
using OrchardCore.Setup.Events;
using OrchardCore.Users.Models;
using OrchardCore.Users.Services;

namespace OrchardCore.CustomSetup.Events
{
    /// <summary>
    /// During setup, creates the admin user account since we never want to give anyone super user.
    /// </summary>
    public class AddAdminUserWhenTenantCreatedEventHandler : ICustomTenantSetupEventHandler
    {
        private readonly IUserService _userService;

        public AddAdminUserWhenTenantCreatedEventHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task Setup(string email, string password, string role, Action<string, string> reportError)
        {
            var user = new User
            {
                UserName = email,
                Email = email,
                RoleNames = new string[] { role },//Specific case
                EmailConfirmed = true
            };

            return _userService.CreateUserAsync(user, password, reportError);
        }
    }
}
