using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;
using Dr.meow.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;



var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();
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

// ⭐ Google Authentication（重點！）
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleOAuth:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleOAuth:ClientSecret"];

    // 登入成功後 Google 回傳資料的 scope
    options.Scope.Add("email");
    options.Scope.Add("profile");

    // 回呼網址（Google 授權後會跳回這裡）
    options.CallbackPath = "/signin-google";
});


// DbContext
builder.Services.AddDbContext<DrMeowDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DrMeowConnection")
        ?? throw new InvalidOperationException("Connection string 'DrMeowConnection' not found.")
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

// 如果之後有 Identity 可以保留，現在也不會壞
app.UseAuthentication();
app.UseAuthorization();

// ⭐ 一定要在 MapRazorPages 之前
app.UseSession();

// Razor Pages 路由
app.MapRazorPages();

// ⭐ 讓根目錄一打開就是 Login
app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.Run();