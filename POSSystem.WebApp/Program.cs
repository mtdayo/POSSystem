using Microsoft.AspNetCore.Components.Authorization;
using POSSystem.WebApp.Components;
using POSSystem.WebApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using POSSystem.WebApp.Components.Pages;
using POSSystem.Infrastructure.Repositories;
using POSSystem.Application.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//Cookie認証を実装
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "POSSystem.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;

        //未認証時はログイン画面ほ以降
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/forbidden";
    });



builder.Services.AddHttpClient<ProductApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WebApiBaseAddress"] ?? "https://localhost:7030/");
});

builder.Services.AddHttpClient<AuthApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WebApiBaseAddress"] ?? "https://localhost:7030/");
});

builder.Services.AddHttpClient<AccountApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WebApiBaseAddress"] ?? "https://localhost:7030");
});

//従業員用ポリシーを登録
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeeOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("isEmployee", "True");
    });
});
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddControllersWithViews();


builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<CartRepository>();
builder.Services.AddScoped<PurchaseRepository>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<SaleRepository>();
builder.Services.AddScoped<SalesService>();
builder.Services.AddScoped<BalanceStateService>();


//認証状態
builder.Services.AddAuthenticationCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
