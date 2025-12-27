using System.Text.Json.Serialization;
using FuFood.Data;
using FuFood.Models.Enums;
using FuFood.Repositories;
using FuFood.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// 添加 DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDbContext"),
        o => { o.MapEnum<UnitType>("product_unit"); }));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 添加 Controller
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configuration
builder.Services.Configure<LineOAuthOptions>(
    builder.Configuration.GetSection(LineOAuthOptions.SectionName));
builder.Services.Configure<CryptoOptions>(
    builder.Configuration.GetSection(CryptoOptions.SectionName));

// Repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RevokedAccessTokenRepository>();
builder.Services.AddScoped<RefrigeratorRepository>();
builder.Services.AddScoped<InventoryTransactionRepository>();
builder.Services.AddScoped<InventoryTransactionItemRepository>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<InventoryQueryRepository>();

// Services
builder.Services.AddHttpClient<LineOAuthService>();
builder.Services.AddSingleton<CryptoService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<RefrigeratorService>();
builder.Services.AddScoped<CommitTransactionService>();

builder.Services.AddAuthentication("AccessToken")
    .AddScheme<AccessTokenHandler.AccessTokenHandlerOptions, AccessTokenHandler>("AccessToken", opts => { });

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendAppPolicy", builder =>
    {
        builder
            .WithOrigins(["http://localhost:5173", "https://fufood.jocelynh.me"])
            .AllowAnyHeader() // 允許瀏覽器附上任何請求標頭
            .AllowAnyMethod() // 允許 get, post 等,任何 http 請求方法
            .AllowCredentials(); //允許瀏覽器傳送 Http only cookies
    });
});

// Swagger 設定 - 必須保留 AddSwaggerGen 讓 SwaggerUI 可以運作
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("FrontendAppPolicy");

app.UseAuthentication();
app.UseAuthorization();

// 使用自定義 OpenAPI JSON 文件
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
});

// 覆寫預設的 swagger.json，提供自定義的 OpenAPI 文件
app.MapGet("/swagger/v1/swagger.json", async context =>
{
    var jsonPath = Path.Combine(AppContext.BaseDirectory, "swagger.json");
    if (File.Exists(jsonPath))
    {
        context.Response.ContentType = "application/json";
        await context.Response.SendFileAsync(jsonPath);
    }
    else
    {
        context.Response.StatusCode = 404;
    }
}).AllowAnonymous();

// 也保留 /api/openapi.json 作為備用
app.MapGet("/api/openapi.json", async context =>
{
    var jsonPath = Path.Combine(AppContext.BaseDirectory, "swagger.json");
    if (File.Exists(jsonPath))
    {
        context.Response.ContentType = "application/json";
        await context.Response.SendFileAsync(jsonPath);
    }
    else
    {
        context.Response.StatusCode = 404;
    }
}).AllowAnonymous();

app.MapOpenApi();

if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (args.Contains("seed"))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await Seeds.Run(db);
    }

    Environment.Exit(0);
}

app.MapControllers().RequireAuthorization();
app.MapSwagger();

app.Run();