using System;
using System.Threading.Tasks;
using OrchardCore.Data.Migration;
using OrchardCore.Recipes.Services;

namespace LefeWareLearning.Tenants
{
    public class CustomSetupMigrations : DataMigration
    {
        private readonly IRecipeMigrator _recipeMigrator;

        public CustomSetupMigrations(IRecipeMigrator recipeMigrator)
        {
            _recipeMigrator = recipeMigrator;
        }

        //public async Task<int> CreateAsync()
        //{
        //    //await _recipeMigrator.ExecuteAsync("NewTenantCreatedWorkflow.recipe.json", this);
        //    //return await Task.FromResult(1);
        //}
    }
}
