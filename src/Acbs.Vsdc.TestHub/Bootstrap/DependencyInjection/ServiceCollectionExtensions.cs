using Acbs.Vsdc.TestHub.Data;
using Acbs.Vsdc.TestHub.Modules.Msp.Builders;
using Acbs.Vsdc.TestHub.Modules.Msp.Encoding;
using Acbs.Vsdc.TestHub.Modules.Msp.Parsing;
using Acbs.Vsdc.TestHub.Modules.Msp.Persistence;
using Acbs.Vsdc.TestHub.Modules.Msp.Reports;
using Acbs.Vsdc.TestHub.Modules.Msp.Simulator;
using Acbs.Vsdc.TestHub.Modules.Msp.Validation;
using Acbs.Vsdc.TestHub.Options;
using Acbs.Vsdc.TestHub.Services.Files;
using Acbs.Vsdc.TestHub.Services.Fin;
using Acbs.Vsdc.TestHub.Services.Simulator;
using Acbs.Vsdc.TestHub.Services.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Acbs.Vsdc.TestHub.Services.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Acbs.Vsdc.TestHub.Bootstrap;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTestHubServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IPasswordHashService, Pbkdf2PasswordHashService>();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = configuration.GetValue<string>("Authentication:CookieName") ?? "ACBS_VSDC_TESTHUB_AUTH";
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(configuration.GetValue<int?>("Authentication:SessionHours") ?? 10);
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole(RoleCodes.Admin));
            options.AddPolicy("TesterOrAdmin", policy => policy.RequireRole(RoleCodes.Admin, RoleCodes.Tester));
            options.AddPolicy("ViewerOrAbove", policy => policy.RequireRole(RoleCodes.Admin, RoleCodes.Tester, RoleCodes.Viewer));
        });

        services.AddControllersWithViews(options => options.Filters.Add(new AuthorizeFilter()));
        services.AddOptions<GatewayFolderOptions>().Bind(configuration.GetSection(ConfigurationSectionNames.GatewayFolders)).ValidateOnStart();
        services.AddOptions<SimulatorOptions>().Bind(configuration.GetSection(ConfigurationSectionNames.Simulator)).ValidateOnStart();
        services.AddOptions<MspOptions>().Bind(configuration.GetSection(ConfigurationSectionNames.Msp)).ValidateOnStart();
        services.AddOptions<DatabaseOptions>().Bind(configuration.GetSection(ConfigurationSectionNames.Database)).ValidateOnStart();
        services.AddSingleton<IValidateOptions<GatewayFolderOptions>, GatewayFolderOptionsValidator>();
        services.AddSingleton<IValidateOptions<MspOptions>, MspOptionsValidator>();

        var database = configuration.GetSection(ConfigurationSectionNames.Database).Get<DatabaseOptions>() ?? new();
        var connection = configuration.GetConnectionString("VsdcDb") ?? throw new InvalidOperationException("Thiếu ConnectionStrings:VsdcDb");
        services.AddDbContext<VsdcDbContext>(options =>
        {
            options.UseSqlServer(connection, sql =>
            {
                sql.EnableRetryOnFailure();
                sql.CommandTimeout(database.CommandTimeoutSeconds);
            });
            if (environment.IsDevelopment() && database.EnableSensitiveDataLogging) options.EnableSensitiveDataLogging();
        });

        services.AddSingleton<FinParser>();
        services.AddSingleton<FinMessageBuilder>();
        services.AddSingleton<SimulatorRuntimeState>();
        services.AddSingleton<IMessageSequenceProvider, InMemoryMessageSequenceProvider>();
        services.AddSingleton<IVietnameseSwiftCodec, VietnameseSwiftCodec>();
        services.AddSingleton<IMspMessageClassifier, MspMessageClassifier>();
        services.AddSingleton<IMspMessageValidator, MspMessageValidator>();
        services.AddSingleton<MspEnvelopeBuilder>();
        services.AddSingleton<IMspMessageBuilderFactory, MspMessageBuilderFactory>();
        services.AddSingleton<MspAckNakBuilder>();
        services.AddSingleton<MspReconcileReportBuilder>();

        services.AddScoped<IMspPersistenceService, MspPersistenceService>();
        services.AddScoped<IMspAutoResponseCoordinator, MspAutoResponseCoordinator>();
        services.AddScoped<IFileIngestionService, FileIngestionService>();
        services.AddScoped<OutgoingMessageService>();
        services.AddScoped<OutgoingMspMessageService>();
        services.AddScoped<SimulatorService>();

        services.AddHostedService<ReceiveFolderWorker>();
        services.AddHostedService<SimulatorSendWorker>();
        return services;
    }
}
