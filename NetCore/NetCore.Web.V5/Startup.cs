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
            #region ���ǳ���A
            Common.SetDataProtection(services, @"D:\DataProtector\", "NetCore", Enums.CryptoType.CngCbc);

            //������                         ���빰
            //IUser �������̽��� UserService Ŭ������ �ޱ� ���� services�� ����ؾ� ��.
            services.AddScoped<DBFirstDbInitializer>();
            services.AddScoped<IUser, UserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddHttpContextAccessor();

            //DB��������, Migrations ������Ʈ ����
            //services.AddDbContext<CodeFirstDbContext>(options =>
            //            options.UseSqlServer(connectionString: Configuration.GetConnectionString(name: "DefaultConnection"),
            //                                 sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations")));

            //DB����������
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

            // .Net Core 2.1�� AddMvc()���� ������ ���� �޼������ �����. 
            services.AddControllersWithViews();

            #region ���ǳ���B
            //�ſ������� ���α���
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
                //���� ���ѽð�
                options.IdleTimeout = TimeSpan.FromMinutes(30); //�⺻���� 20��
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

            // �Ʒ��� app.UseEndpoints()�޼��带 ����ð� �����ϱ� ���� ����.
            app.UseRouting();

            // ���α����� ����ϱ� ���� �߰���.
            app.UseAuthorization();

            #region ���ǳ���
            //�ſ�������
            app.UseAuthentication();

            //���� ����
            //System.InvalidOperationException:
            //'Session has not been configured for this application or request.'
            app.UseSession();
            #endregion

            // .Net Core 2.1�� UseMvc()���� ������ ���� �޼������ �����. 
            app.UseEndpoints(endpoints =>
            {
                // .Net Core 2.1�� UseMvc()���� ������ ���� �޼������ �����.
                endpoints.MapControllerRoute(
                    name: "default",
                    // .Net Core 2.1�� template���� ������ ���� �Ķ���͸��� �����.
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
