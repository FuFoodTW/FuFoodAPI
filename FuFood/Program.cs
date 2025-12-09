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