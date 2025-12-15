using FuFood.Data;
using FuFood.Repositories;
using FuFood.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 添加 DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDbContext")));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 添加 Controller
builder.Services.AddControllersWithViews();
builder.Services.Configure<LineOAuthOptions>(
    builder.Configuration.GetSection(LineOAuthOptions.SectionName));
builder.Services.Configure<CryptoOptions>(
    builder.Configuration.GetSection(CryptoOptions.SectionName));
builder.Services.AddHttpClient<LineOAuthService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddSingleton<CryptoService>();
builder.Services.AddSingleton<JwtService>();

builder.Services.AddAuthentication("AccessToken")
    .AddScheme<AccessTokenHandler.AccessTokenHandlerOptions, AccessTokenHandler>("AccessToken", opts => { });

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendAppPolicy", builder =>
    {
        builder
            .WithOrigins(["http://localhost:5173", "https://fufood.vercel.app"])
            .AllowAnyHeader() // 允許瀏覽器附上任何請求標頭
            .AllowAnyMethod() // 允許 get, post 等,任何 http 請求方法
            .AllowCredentials(); //允許瀏覽器傳送 Http only cookies
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.MapControllers().RequireAuthorization();

app.Run();