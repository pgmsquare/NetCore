using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCore.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Web.V31
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            #region 강의내용
            var webHost = CreateHostBuilder(args).Build();

            using (var scope = webHost.Services.CreateScope())
            {
                DBFirstDbInitializer initializer = scope.ServiceProvider
                                                        .GetService<DBFirstDbInitializer>();

                int rowAffected = initializer.PlantSeedData();
            }

            webHost.Run();
            #endregion
        }

        // .Net Core 2.1의 IWebHostBuilder에서 다음과 같이 메서드 type이 변경됨.
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            // .Net Core 2.1의 WebHost에서 다음과 같이 메서드 type이 변경됨.
            Host.CreateDefaultBuilder(args)
                #region 강의내용
                .ConfigureLogging(builder => builder.AddFile(options =>
                {
                    options.LogDirectory = "Logs";      //로그저장폴더
                    options.FileName = "log-";          //로그파일접두어. log-20180000.txt
                    options.FileSizeLimit = null;       //로그파일 사이즈 제한 (10MB)
                    options.RetainedFileCountLimit = null;  //로그파일 보유갯수 (2)
                }))
        #endregion
                // .Net Core 2.1의 .UseStartup<Startup>()이 다음과 같이
                // Lambda식으로 변경됨.
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
