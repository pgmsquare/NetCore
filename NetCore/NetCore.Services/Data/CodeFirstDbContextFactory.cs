using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NetCore.Services.Config;
using System.Configuration;

namespace NetCore.Services.Data
{
    // Microsoft.EntityFrameworkCore.Design NuGet Package 2.1.2
    public class CodeFirstDbContextFactory : IDesignTimeDbContextFactory<CodeFirstDbContext>
    {
        private const string _configPath = @"D:\Repository\pgmsquare\NetCore\NetCore\NetCore.Web.V5\appsettings.json";

        public CodeFirstDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CodeFirstDbContext>();

            optionsBuilder.UseSqlServer(new DbConnector(_configPath).GetConnectionString());

            return new CodeFirstDbContext(optionsBuilder.Options);
        }
    }
}
