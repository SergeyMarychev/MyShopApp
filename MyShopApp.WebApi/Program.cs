using Microsoft.OpenApi.Models;
using MyShopApp.Application.Extensions;
using MyShopApp.Domain.Roles;
using MyShopApp.Domain.Users;
using MyShopApp.Infrastructure;
using MyShopApp.Infrastructure.Extensions;
using MyShopApp.WebApi.Filters;
using MyShopApp.WebApi.Localization;
using Serilog;
using Serilog.Formatting.Json;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.File(path: "logs/bootstrap-.ndjson", formatter: new JsonFormatter(), rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .MinimumLevel.Debug()
    .CreateBootstrapLogger();

var logger = Log.Logger;

try
{
    logger.Information("Запуск {ApplicationName}...", "MyShopApp");

    var builder = WebApplication.CreateBuilder(args);

    logger.Information("Конфигурация хоста...");

    var configuration = builder.Configuration;

    builder.Services.AddInfrastructureServices(configuration);
    builder.Services.AddApplicationServices(configuration);

    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    builder.Services.AddControllers(config =>
    {
        config.Filters.Add<TransactionFilter>();
        config.Filters.Add<ExceptionFilter>();
        config.Filters.Add<ResultFilter>();
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        // Bearer token authentication
        OpenApiSecurityScheme securityDefinition = new OpenApiSecurityScheme()
        {
            Name = "Bearer",
            BearerFormat = "JWT",
            Scheme = "bearer",
            Description = "Specify the authorization token.",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
        };
        c.AddSecurityDefinition("jwt_auth", securityDefinition);

        // Make sure swagger UI requires a Bearer token specified
        OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme()
        {
            Reference = new OpenApiReference()
            {
                Id = "jwt_auth",
                Type = ReferenceType.SecurityScheme
            }
        };
        OpenApiSecurityRequirement securityRequirements = new OpenApiSecurityRequirement()
    {
        {securityScheme, new string[] { }},
    };
        c.AddSecurityRequirement(securityRequirements);
    });

    //Identity
    builder.Services
        .AddIdentity<User, Role>(options =>
        {
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 2;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddErrorDescriber<MultiLanguageIdentityErrorDescriber>();

    builder.Logging.ClearProviders();

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            ;
    });

    builder.Logging.AddConsole();


    logger.Information("Запуск хоста...");

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.UseCors(x => x.AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowAnyOrigin());

    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Не удалось запустить приложение.");
}
finally
{
    Log.CloseAndFlush();
}