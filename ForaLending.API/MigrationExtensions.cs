using ForaLending.API.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace ForaLending.API;

    public static class MigrationExtensions
    {

        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using ForaFinancialContext dbContext = scope.ServiceProvider.GetRequiredService<ForaFinancialContext>();

            dbContext.Database.Migrate();
        }
    }

