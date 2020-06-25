using System;
using System.Threading.Tasks;

namespace LefeWareLearning.CustomSetup.Events
{
    /// <summary>
    /// Called when a tenant is set up.
    /// </summary>
    public interface ICustomTenantSetupEventHandler
    {
        Task Setup(string email, string password, Action<string, string> reportError);
    }
}
