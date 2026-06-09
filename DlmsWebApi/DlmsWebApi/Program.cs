using DlmsWebApi;
using DlmsWebApi.Business.AuthorBusiness;
using DlmsWebApi.Caching;
using DlmsWebApi.Extensions.BasicAuthentication;
using DlmsWebApi.Repository.AuthorRepository;
using DlmsWebApi.Repository.Data;
using DlmsWebApi.Repository.RepositoryPattern;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);


// 1. Add CORS services to the container
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8000") // Or Use AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy(AuthorCacheKeys.AuthorListOutputCachePolicy, policy =>
        policy
            .Expire(TimeSpan.FromSeconds(60))
            .SetVaryByQuery("api-version")
            .Tag(AuthorCacheKeys.AuthorTag));

    options.AddPolicy(AuthorCacheKeys.AuthorDetailsOutputCachePolicy, policy =>
        policy
            .Expire(TimeSpan.FromSeconds(60))
            .SetVaryByQuery("id", "api-version")
            .Tag(AuthorCacheKeys.AuthorTag));
});

// Add rate limiting policies (demonstrate all main limiter types)
builder.Services.AddRateLimiter(options =>
{
    // 1) Token-bucket per remote IP (good for general request smoothing)
    options.AddPolicy("Author:TokenBucket", context =>
        RateLimitPartition.GetTokenBucketLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 20, // maximum burst size
                TokensPerPeriod = 10, // tokens added each period
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 5
            }));

    // 2) Fixed-window per IP (simple rate by window)
    options.AddPolicy("Author:FixedWindow", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 2, // permits per window
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // 3) Sliding-window per IP (more even distribution than fixed)
    options.AddPolicy("Author:SlidingWindow", context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 10,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // 4) Concurrency limiter for write operations (limits concurrent executions)
    options.AddPolicy("Author:Concurrency", context =>
        RateLimitPartition.GetConcurrencyLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            _ => new ConcurrencyLimiterOptions
            {
                PermitLimit = 2, // allows 2 concurrent write operations per client
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            }));

    // 5) API-key partitioned fixed-window (partition by X-Api-Key header when present)
    options.AddPolicy("Author:PerApiKeyFixedWindow", context =>
    {
        // Choose API key if present, otherwise fallback to IP
        var partitionKey = context.Request.Headers.TryGetValue("X-Api-Key", out var values) && !StringValues.IsNullOrEmpty(values)
            ? values.ToString()
            : context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 60,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    // Global rejection handling: return 429 with Retry-After header and a short body
    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        if (!context.HttpContext.Response.Headers.ContainsKey("Retry-After"))
        {
            context.HttpContext.Response.Headers.RetryAfter = "60"; // seconds
        }
        await context.HttpContext.Response.WriteAsync("Too Many Requests. Please try again later.", ct);
    };
});

var useRedis = builder.Configuration.GetValue<bool>("Cache:UseRedis");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

if (useRedis && !string.IsNullOrWhiteSpace(redisConnectionString))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = builder.Configuration["Cache:RedisInstanceName"] ?? "DlmsWebApi:";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

// Configure Swagger/OpenAPI generation and enable API-versioned explorer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
 options.UseSqlite(connectionString));

// Add API Versioning Services and register API explorer to provide versioned API metadata
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("x-api-version"),
        new UrlSegmentApiVersionReader()
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddScoped<IBasicAuthService, BasicAuthService>();
builder.Services.AddScoped<IAuthorBusiness, AuthorBusiness>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IAuthorCacheInvalidator, AuthorCacheInvalidator>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger middleware and expose one UI endpoint per API version
    app.UseSwagger();

    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwaggerUI(options =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseResponseCaching();
app.UseOutputCache();
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
