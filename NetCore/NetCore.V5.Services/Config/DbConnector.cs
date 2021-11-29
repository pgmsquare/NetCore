using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.V5.Services.Config
{
    public class DbConnector
    {
        private readonly string _connectionString = string.Empty;

        public DbConnector(string configPath)
        {
            /*
            - Add NuGet Packages
                (1) Microsoft.Extensions.Configuration 2.1.1
                (2) Microsoft.Extensions.Configuration.Abstractions 2.1.1
                (3) Microsoft.Extensions.Configuration.Json 2.1.1
             */
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile(configPath, false);

           //_connectionString =
           //configBuilder.Build()
           //             .GetSection("ConnectionStrings")
           //             .GetSection("DefaultConnection")
           //             .Value;
           _connectionString =
                configBuilder.Build()["ConnectionStrings:DefaultConnection"];
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
