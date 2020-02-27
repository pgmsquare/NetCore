using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Utilities.Utils;

namespace NetCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            Common.SetDataProtection(services, @"D:\DataProtector\", "NetCore", Enums.CryptoType.CngCbc);

            //껍데기                         내용물
            //IUser 인터페이스가 UserService 클래스를 받기 위해 services에 등록해야 함.
            services.AddScoped<DBFirstDbInitializer>();
            services.AddScoped<IUser, UserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddHttpContextAccessor();

            //DB접속정보, Migrations 프로젝트 지정
            //services.AddDbContext<CodeFirstDbContext>(options =>
            //            options.UseSqlServer(connectionString: Configuration.GetConnectionString(name: "DefaultConnection"),
            //                                 sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations")));

            //DB접속정보만
            services.AddDbContext<DBFirstDbContext>(options =>
                        options.UseSqlServer(connectionString: Configuration.GetConnectionString(name: "DBFirstDBConnection")));

            //Logging
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection(key: "Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });

            //닷넷코어는 MVC패턴을 사용하기 위해 의존성주입을 써야하기 때문에
            //MVC를 서비스에 등록.
            //SetCompatibilityVersion은 core 2.1 버전부터 추가됨
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //신원보증과 승인권한
            services.AddAuthentication(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.AccessDeniedPath = "/Membership/Forbidden";
                        options.LoginPath = "/Membership/Login";
                    });

            services.AddAuthorization();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".NetCore.Session";
                //세션 제한시간
                options.IdleTimeout = TimeSpan.FromMinutes(30); //기본값은 20분
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Add parameter : ILoggerFactory factory
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory factory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //신원보증만
            app.UseAuthentication();

            //세션 지정
            //System.InvalidOperationException:
            //'Session has not been configured for this application or request.'
            app.UseSession();

            //MVC 라우트 경로 지정
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
