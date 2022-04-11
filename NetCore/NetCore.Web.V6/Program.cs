using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NetCore.Utilities.Utils;
using NetCore.V5.Services.Data;
using NetCore.V5.Services.Interfaces;
using NetCore.V5.Services.Svcs;

var builder = WebApplication.CreateBuilder(args);
#region 강의내용
builder.Host.ConfigureLogging(logBuilder => logBuilder.AddFile(options =>
{
    options.LogDirectory = "Logs";      //로그저장폴더
    options.FileName = "log-";          //로그파일접두어. log-20180000.txt
    options.FileSizeLimit = null;       //로그파일 사이즈 제한 (10MB)
    options.RetainedFileCountLimit = null;  //로그파일 보유갯수 (2)
}));
#endregion

// Add services to the container.
#region 강의내용A
Common.SetDataProtection(builder.Services, @"D:\DataProtector\", "NetCore", Enums.CryptoType.CngCbc);

//껍데기               내용물
//IUser 인터페이스가 UserService 클래스를 받기 위해 services에 등록해야 함.
builder.Services.AddScoped<DBFirstDbInitializer>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddHttpContextAccessor();

//DB접속정보, Migrations 프로젝트 지정
//builder.Services.AddDbContext<CodeFirstDbContext>(options =>
//            options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString(name: "DefaultConnection"),
//                                 sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.V5.Services")));

//DB접속정보만
builder.Services.AddDbContext<DBFirstDbContext>(options =>
            options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString(name: "DBFirstDBConnection")));

//Logging
builder.Services.AddLogging(logBuilder =>
{
    logBuilder.AddConfiguration(builder.Configuration.GetSection(key: "Logging"));
    logBuilder.AddConsole();
    logBuilder.AddDebug();
});
#endregion
builder.Services.AddControllersWithViews();

#region 강의내용B
//신원보증과 승인권한
builder.Services.AddAuthentication(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.AccessDeniedPath = "/Membership/Forbidden";
            options.LoginPath = "/Membership/Login";
        });

builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".NetCore.Session";
    //세션 제한시간
    options.IdleTimeout = TimeSpan.FromMinutes(30); //기본값은 20분
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

/*⭐⭐⭐⭐⭐⭐⭐⭐
            app.UseRouting(), app.UseAuthentication(), app.UseAuthorization(),
            app.UseSession(), app.MapControllerRoute()
            이렇게 5개의 메서드는 반드시 순서를 지켜야 올바로 작동함.
            ⭐⭐⭐⭐⭐⭐⭐⭐*/

// 아래의 app.MapControllerRoute()메서드를 라우팅과 연결하기 위해 사용됨.
app.UseRouting();

////강의내용
//신원보증만
app.UseAuthentication();

// 승인권한을 사용하기 위해 추가됨.
app.UseAuthorization();

////강의내용
//세션 지정
//System.InvalidOperationException:
//'Session has not been configured for this application or request.'
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    DBFirstDbInitializer initializer = scope.ServiceProvider
                                            .GetService<DBFirstDbInitializer>();

    int rowAffected = initializer.PlantSeedData();
}

app.Run();
