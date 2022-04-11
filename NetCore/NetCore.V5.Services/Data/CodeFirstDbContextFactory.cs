using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NetCore.V5.Services.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.V5.Services.Data
{
    // Microsoft.EntityFrameworkCore.Design NuGet Package 2.1.2
    public class CodeFirstDbContextFactory
        : IDesignTimeDbContextFactory<CodeFirstDbContext>
    {
        /*
         * 조금 불편해도 appsettings.json을 Web Project에서만
         * 관리하도록 하기 위해 수동으로 파일경로를 지정합니다.
         * 수강생 여러분들은 각자의 웹프로젝트 위치로 변경하시면 됩니다.
         */
        private const string _configPath =
                @"D:\Repository\pgmsquare\NetCore\NetCore\NetCore.Web.V5\appsettings.json";

        public CodeFirstDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CodeFirstDbContext>();
            optionsBuilder.UseSqlServer(new DbConnector(_configPath).GetConnectionString());

            return new CodeFirstDbContext(optionsBuilder.Options);
        }
    }
}
