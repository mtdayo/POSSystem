using Microsoft.AspNetCore.Components.Authorization;
using POSSystem.WebApp.Components;
using POSSystem.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<ProductApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WebApiBaseAddress"] ?? "https://localhost:7030/");
});

builder.Services.AddHttpClient<AuthApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WebApiBaseAddress"] ?? "https://localhost:7030/");
});

//依存性注入の設定
builder.Services.AddSingleton<AuthState>(); //テスト用

//builder.Services.AddScoped<AuthState>(); //本番用

//認証状態
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

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

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
