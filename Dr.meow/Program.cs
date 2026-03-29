using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;
using Dr.meow.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Hangfire;
using Hangfire.MemoryStorage; // 👈 記得加


var builder = WebApplication.CreateBuilder(args);

// 註冊 HttpContextAccessor，這樣你才能在 Razor Pages 中使用 @inject
builder.Services.AddHttpContextAccessor();

// Razor Pages
builder.Services.AddRazorPages();

// ⭐ Hangfire 註冊
builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage(); // 測試用（最簡單）
});

builder.Services.AddHangfireServer();

// ⭐ 註冊 RAG 搜尋服務（你的 AI 搜尋）
builder.Services.AddHttpClient<ISearchService, SearchService>();

// ⭐ Session 服務
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ⭐ 只使用 Cookie Authentication（暫時不啟用 Google）
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Error";

    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleOAuth:ClientId"]
        ?? throw new InvalidOperationException("Google ClientId 未設定。");
    options.ClientSecret = builder.Configuration["GoogleOAuth:ClientSecret"]
        ?? throw new InvalidOperationException("Google ClientSecret 未設定。");
    options.Events = new OAuthEvents
    {
        OnRedirectToAuthorizationEndpoint = context =>
        {
            // 在導向 Google 登入頁面的 URL 後面加上 prompt=select_account
            context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
            return Task.CompletedTask;
        }
    };
});   

// DbContext
builder.Services.AddDbContext<DrMeowDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DrMeowDbContext' not found.")
    )
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseHangfireDashboard(); // 可選，但建議加

// ⭐ Session 必須放在 UseRouting 之後，UseAuthorization 之前
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// ⭐ 讓根目錄直接跳轉 Login
app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.Run();
