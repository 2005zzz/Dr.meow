using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// ⭐ 服務註冊區 (DI Container)
// =====================================

// Razor Pages
builder.Services.AddRazorPages();

// ⭐ Session 使用者登入狀態管理
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ⭐ DbContext
builder.Services.AddDbContext<DrMeowDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DrMeowDbContext")
        ?? throw new InvalidOperationException("Connection string 'DrMeowDbContext' not found.")
    )
);

var app = builder.Build();

// =====================================
// ⭐ Middleware (執行管線)
// =====================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 若未來加 Identity 或 JWT 可用
app.UseAuthentication();
app.UseAuthorization();

// ⭐ 必須在 MapRazorPages() 前
app.UseSession();

// Razor Pages Routing
app.MapRazorPages();

// ⭐ 預設首頁 → Login
app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.Run();
