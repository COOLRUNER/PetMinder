using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using PetMinder.Api;
using PetMinder.Data;
using WebApplication1.Services;
using WebApplication1.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SignalR;
using PetMinder.Api.Hubs;
using PetMinder.Api.Services;

var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString) || connectionString == "ConnectionStringHere")
{
    connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Could not find a valid connection string.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PetMinder API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// rate limiter for IP throttling
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "registration", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddHttpClient();

var emailUsername = builder.Configuration["Email:Username"];
var emailPassword = builder.Configuration["Email:Password"];
var fromAddress = builder.Configuration["Email:FromAddress"];

builder.Services.AddFluentEmail(fromAddress)
    .AddSmtpSender(() => new SmtpClient("smtp-relay.brevo.com")
    {
        Port = 2525,
        Credentials = new NetworkCredential(emailUsername, emailPassword),
        EnableSsl = true,
        Timeout = 10000 // 10 seconds timeout
    });


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IQualificationService, QualificationService>();
builder.Services.AddScoped<IRestrictionService, RestrictionService>();
builder.Services.AddScoped<ISitterAvailabilityService, SitterAvailabilityService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPointsService, PointsService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IGeocodingService, GeocodingService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IGamificationService, GamificationService>();
builder.Services.AddScoped<IReferralService, ReferralService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IDatabaseMaintenanceService, DatabaseMaintenanceService>();

builder.Services.AddSignalR();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var frontendUrl = builder.Configuration["Frontend:BaseUrl"];
var envFrontend = Environment.GetEnvironmentVariable("FRONTEND_URL");

if (!string.IsNullOrWhiteSpace(envFrontend))
{
    frontendUrl = envFrontend;
    var primaryUrl = envFrontend.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                               .FirstOrDefault()?.Trim().TrimEnd('/');
    
    if (!string.IsNullOrEmpty(primaryUrl))
    {
        builder.Configuration["Frontend:BaseUrl"] = primaryUrl;
    }
}

if (string.IsNullOrWhiteSpace(frontendUrl))
{
     frontendUrl = "https://localhost:7185";
}

var origins = frontendUrl.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(o => o.Trim().TrimEnd('/'))
                         .ToList();

// Add common variants automatically if not present
var additionalOrigins = new List<string>();
foreach (var origin in origins)
{
    if (origin.StartsWith("https://") && !origin.Contains("www.") && !origin.Contains("localhost"))
    {
        additionalOrigins.Add(origin.Replace("https://", "https://www."));
    }
    else if (origin.Contains("www."))
    {
        additionalOrigins.Add(origin.Replace("www.", ""));
    }
}
origins.AddRange(additionalOrigins);
origins = origins.Distinct().ToList();

Console.WriteLine($"CORS: Allowing origins: {string.Join(", ", origins)}");

builder.Services.AddCors(options =>
{
    if (!string.IsNullOrWhiteSpace(envFrontend))
    {
        frontendUrl = envFrontend;
        var primaryUrl = envFrontend.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                   .FirstOrDefault()?.Trim().TrimEnd('/');
        
        if (!string.IsNullOrEmpty(primaryUrl))
        {
            builder.Configuration["Frontend:BaseUrl"] = primaryUrl;
        }
    }
    
    if (string.IsNullOrWhiteSpace(frontendUrl))
    {
         frontendUrl = "https://localhost:7185";
    }

    var origins = frontendUrl.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(o => o.Trim().TrimEnd('/'))
                             .ToList();

    var additionalOrigins = new List<string>();
    foreach (var origin in origins)
    {
        if (origin.StartsWith("https://") && !origin.Contains("www.") && !origin.Contains("localhost"))
        {
            additionalOrigins.Add(origin.Replace("https://", "https://www."));
        }
        else if (origin.Contains("www."))
        {
            additionalOrigins.Add(origin.Replace("www.", ""));
        }
    }
    origins.AddRange(additionalOrigins);
    origins = origins.Distinct().ToList();

    Console.WriteLine($"CORS: Allowing origins: {string.Join(", ", origins)}");

    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(origins.ToArray())
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // Required for SignalR
        });
});

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(connectionString));

builder.Services.AddHangfireServer();

var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<IPointsService>(
    "expire-points",
    service => service.ExpirePointsAsync(),
    Cron.Daily(2)
);

RecurringJob.AddOrUpdate<IDatabaseMaintenanceService>(
    "db-keep-alive",
    service => service.PokeDatabaseAsync(),
    "*/4 * * * *"
);

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/healthz");

app.MapHub<ChatHub>("/chathub");

app.Run();