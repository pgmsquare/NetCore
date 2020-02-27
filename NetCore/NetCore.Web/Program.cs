using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCore.Services.Data;

namespace NetCore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();
            var webHost = BuildWebHost(args);

            using (var scope = webHost.Services.CreateScope())
            {
                DBFirstDbInitializer initializer = scope.ServiceProvider
                                                        .GetService<DBFirstDbInitializer>();

                int rowAffected = initializer.PlantSeedData();
            }

            webHost.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(builder => builder.AddFile(options =>
                {
                    options.LogDirectory = "Logs";      //로그저장폴더
                    options.FileName = "log-";          //로그파일접두어. log-20180000.txt
                    options.FileSizeLimit = null;       //로그파일 사이즈 제한 (10MB)
                    options.RetainedFileCountLimit = null;  //로그파일 보유갯수 (2)
                }))
                .UseStartup<Startup>()
                .Build();

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>();
    }
}
