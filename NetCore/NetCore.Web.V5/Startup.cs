using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Utilities.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Web.V5
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
            #region 강의내용A
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
            #endregion

            // .Net Core 2.1의 AddMvc()에서 다음과 같이 메서드명이 변경됨. 
            services.AddControllersWithViews();

            #region 강의내용B
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
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            /*⭐⭐⭐⭐⭐⭐⭐⭐
            app.UseRouting(), app.UseAuthentication(), app.UseAuthorization(),
            app.UseSession(), app.UseEndpoints()
            이렇게 5개의 메서드는 반드시 순서를 지켜야 올바로 작동함.
            ⭐⭐⭐⭐⭐⭐⭐⭐*/

            // 아래의 app.UseEndpoints()메서드를 라우팅과 연결하기 위해 사용됨.
            app.UseRouting();

            ////강의내용
            //신원보증만
            app.UseAuthentication();

            // 권한을 승인하기 위해 메서드가 추가됨.
            app.UseAuthorization();

            ////강의내용
            //세션 지정
            //System.InvalidOperationException:
            //'Session has not been configured for this application or request.'
            app.UseSession();

            // .Net Core 2.1의 UseMvc()에서 다음과 같이 메서드명이 변경됨. 
            app.UseEndpoints(endpoints =>
            {
                // .Net Core 2.1의 UseMvc()에서 다음과 같이 메서드명이 변경됨.
                endpoints.MapControllerRoute(
                    name: "default",
                    // .Net Core 2.1의 template에서 다음과 같이 파라미터명이 변경됨.
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
