using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;
using Dr.meow.Services;

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
