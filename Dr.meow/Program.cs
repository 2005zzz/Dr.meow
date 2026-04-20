using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;
using Dr.meow.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Hangfire;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);

// 註冊 HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// Razor Pages
builder.Services.AddRazorPages();

// Hangfire
builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
});
builder.Services.AddHangfireServer();

// RAG 搜尋服務
builder.Services.AddHttpClient<ISearchService, SearchService>();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Azure OpenAI：有填設定才啟用
var azureEndpoint = builder.Configuration["AzureOpenAI:Endpoint"];
var azureApiKey = builder.Configuration["AzureOpenAI:ApiKey"];
var azureDeploymentName = builder.Configuration["AzureOpenAI:DeploymentName"];

if (!string.IsNullOrWhiteSpace(azureEndpoint) &&
    !string.IsNullOrWhiteSpace(azureApiKey) &&
    !string.IsNullOrWhiteSpace(azureDeploymentName))
{
    builder.Services.AddAzureOpenAIChatCompletion(
        deploymentName: azureDeploymentName,
        apiKey: azureApiKey,
        endpoint: azureEndpoint,
        modelId: "gpt-5.2-chat"
    );
}

// Authentication
var authBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

authBuilder.AddCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Error";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Google OAuth：有填 ClientId + ClientSecret 才啟用
var googleClientId = builder.Configuration["GoogleOAuth:ClientId"];
var googleClientSecret = builder.Configuration["GoogleOAuth:ClientSecret"];

if (!string.IsNullOrWhiteSpace(googleClientId) &&
    !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.Events = new OAuthEvents
        {
            OnRedirectToAuthorizationEndpoint = context =>
            {
                context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
                return Task.CompletedTask;
            }
        };
    });
}

// DbContext
builder.Services.AddDbContext<DrMeowDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
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

app.UseHangfireDashboard();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/Login");
    return Task.CompletedTask;
});
app.MapControllers();

app.Run();