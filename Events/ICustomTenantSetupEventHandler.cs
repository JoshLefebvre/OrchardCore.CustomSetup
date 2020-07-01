using System;
using System.Threading.Tasks;

namespace OrchardCore.CustomSetup.Events
{
    /// <summary>
    /// Called when a tenant is set up.
    /// </summary>
    public interface ICustomTenantSetupEventHandler
    {
        Task Setup(string email, string password, string role, Action<string, string> reportError);
    }
}
