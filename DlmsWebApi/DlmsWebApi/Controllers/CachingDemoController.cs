using System.Text.Json;
using DlmsWebApi.Business.AuthorBusiness;
using DlmsWebApi.Caching;
using DlmsWebApi.Shared;
using DlmsWebApi.Shared.AuthorData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using LegacyCacheItemPolicy = System.Runtime.Caching.CacheItemPolicy;
using LegacyMemoryCache = System.Runtime.Caching.MemoryCache;

namespace DlmsWebApi.Controllers;

[ApiController]
[Route("api/cache-demo")]
public class CachingDemoController : ControllerBase
{
    private const string DemoEtag = "\"cache-demo-v1\"";
    private const string LegacyCacheKey = "legacy:cache-demo:time";

    private readonly IAuthorBusiness _authorBusiness;
    private readonly IAuthorCacheInvalidator _authorCacheInvalidator;
    private readonly IDistributedCache _distributedCache;
    private readonly IMemoryCache _memoryCache;

    public CachingDemoController(
        IAuthorBusiness authorBusiness,
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        IAuthorCacheInvalidator authorCacheInvalidator)
    {
        _authorBusiness = authorBusiness;
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _authorCacheInvalidator = authorCacheInvalidator;
    }

    [HttpGet("authors/memory")]
    [OutputCache(PolicyName = AuthorCacheKeys.AuthorListOutputCachePolicy)]
    public async Task<IActionResult> GetAuthorsUsingMemoryCache()
    {
        if (_memoryCache.TryGetValue(AuthorCacheKeys.AuthorListMemory, out List<AuthorDetails>? authors))
        {
            Response.Headers.Append("X-Cache", "Memory HIT");
            return Ok(BuildAuthorCacheResponse("IMemoryCache", true, authors));
        }

        authors = await _authorBusiness.GetList();

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(1),
            Priority = CacheItemPriority.Normal
        };

        _memoryCache.Set("authors:list:memory", authors, cacheOptions);
        Response.Headers.Append("X-Cache", "Memory MISS");

        return Ok(BuildAuthorCacheResponse("IMemoryCache", false, authors));
    }

    [HttpGet("authors/distributed")]
    public async Task<IActionResult> GetAuthorsUsingDistributedCache(CancellationToken cancellationToken)
    {
        var cachedJson = await _distributedCache.GetStringAsync(
            AuthorCacheKeys.AuthorListDistributed,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(cachedJson))
        {
            var cachedAuthors = JsonSerializer.Deserialize<List<AuthorDetails>>(cachedJson) ?? new List<AuthorDetails>();
            Response.Headers.Append("X-Cache", "Distributed HIT");
            return Ok(BuildAuthorCacheResponse("IDistributedCache", true, cachedAuthors));
        }

        var authors = await _authorBusiness.GetList();
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        await _distributedCache.SetStringAsync(
            AuthorCacheKeys.AuthorListDistributed,
            JsonSerializer.Serialize(authors),
            cacheOptions,
            cancellationToken);

        Response.Headers.Append("X-Cache", "Distributed MISS");

        return Ok(BuildAuthorCacheResponse("IDistributedCache", false, authors));
    }

    [HttpGet("authors/hybrid")]
    public async Task<IActionResult> GetAuthorsUsingHybridPattern(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue(AuthorCacheKeys.AuthorListHybrid, out List<AuthorDetails>? memoryAuthors))
        {
            Response.Headers.Append("X-Cache", "Hybrid L1 Memory HIT");
            return Ok(BuildAuthorCacheResponse("Hybrid cache pattern - L1 memory", true, memoryAuthors));
        }

        var cachedJson = await _distributedCache.GetStringAsync(AuthorCacheKeys.AuthorListHybrid, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cachedJson))
        {
            var distributedAuthors = JsonSerializer.Deserialize<List<AuthorDetails>>(cachedJson) ?? new List<AuthorDetails>();
            _memoryCache.Set(
                AuthorCacheKeys.AuthorListHybrid,
                distributedAuthors,
                TimeSpan.FromMinutes(1));

            Response.Headers.Append("X-Cache", "Hybrid L2 Distributed HIT");
            return Ok(BuildAuthorCacheResponse("Hybrid cache pattern - L2 distributed", true, distributedAuthors));
        }

        var authors = await _authorBusiness.GetList();

        _memoryCache.Set(
            AuthorCacheKeys.AuthorListHybrid,
            authors,
            TimeSpan.FromMinutes(1));

        await _distributedCache.SetStringAsync(
            AuthorCacheKeys.AuthorListHybrid,
            JsonSerializer.Serialize(authors),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        Response.Headers.Append("X-Cache", "Hybrid MISS");

        return Ok(BuildAuthorCacheResponse("Hybrid cache pattern - database", false, authors));
    }

    [HttpDelete("authors")]
    public async Task<IActionResult> ClearAuthorCaches(CancellationToken cancellationToken)
    {
        await _authorCacheInvalidator.ClearAsync(cancellationToken);

        return Ok(ApiResponse<object>.SuccessMessage(new
        {
            RemovedMemoryKeys = AuthorCacheKeys.MemoryKeys,
            RemovedDistributedKeys = AuthorCacheKeys.DistributedKeys,
            RemovedOutputCacheTag = AuthorCacheKeys.AuthorTag
        }, "Author cache entries cleared"));
    }

    [HttpGet("response/time")]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
    public IActionResult GetResponseCachedTime()
    {
        return Ok(ApiResponse<object>.SuccessMessage(new
        {
            CacheType = "Response caching middleware",
            ServerGeneratedAtUtc = DateTimeOffset.UtcNow,
            TryAgainWithinSeconds = 30
        }));
    }

    [HttpGet("output/time")]
    [OutputCache(Duration = 30, VaryByQueryKeys = new[] { "name" })]
    public IActionResult GetOutputCachedTime([FromQuery] string? name)
    {
        return Ok(ApiResponse<object>.SuccessMessage(new
        {
            CacheType = "Output cache",
            Name = string.IsNullOrWhiteSpace(name) ? "anonymous" : name,
            ServerGeneratedAtUtc = DateTimeOffset.UtcNow,
            TryAgainWithinSeconds = 30
        }));
    }

    [HttpGet("http-headers")]
    public IActionResult GetHttpHeaderCacheDemo()
    {
        if (Request.Headers.IfNoneMatch.Any(value => string.Equals(value, DemoEtag, StringComparison.Ordinal)))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        Response.Headers.CacheControl = "public, max-age=60";
        Response.Headers.ETag = DemoEtag;
        Response.Headers.LastModified = "Mon, 01 Jan 2024 00:00:00 GMT";

        return Ok(ApiResponse<object>.SuccessMessage(new
        {
            CacheType = "HTTP cache headers",
            CacheControl = "public, max-age=60",
            ETag = DemoEtag,
            ServerGeneratedAtUtc = DateTimeOffset.UtcNow
        }));
    }

    [HttpGet("legacy-object-cache/time")]
    public IActionResult GetLegacyObjectCacheTime()
    {
        var cachedTime = LegacyMemoryCache.Default.Get(LegacyCacheKey) as string;
        if (cachedTime is not null)
        {
            Response.Headers.Append("X-Cache", "Legacy ObjectCache HIT");
            return Ok(BuildLegacyCacheResponse(true, cachedTime));
        }

        cachedTime = DateTimeOffset.UtcNow.ToString("O");
        LegacyMemoryCache.Default.Set(
            LegacyCacheKey,
            cachedTime,
            new LegacyCacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(30)
            });

        Response.Headers.Append("X-Cache", "Legacy ObjectCache MISS");

        return Ok(BuildLegacyCacheResponse(false, cachedTime));
    }

    [HttpGet("no-cache")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult GetNoCacheDemo()
    {
        return Ok(ApiResponse<object>.SuccessMessage(new
        {
            CacheType = "No-store response",
            ServerGeneratedAtUtc = DateTimeOffset.UtcNow,
            Note = "This endpoint tells browsers and proxies not to store the response."
        }));
    }

    private static ApiResponse<object> BuildAuthorCacheResponse(
        string cacheType,
        bool cacheHit,
        List<AuthorDetails>? authors)
    {
        return ApiResponse<object>.SuccessMessage(new
        {
            CacheType = cacheType,
            CacheStatus = cacheHit ? "HIT" : "MISS",
            AuthorCount = authors?.Count ?? 0,
            ServerGeneratedAtUtc = DateTimeOffset.UtcNow,
            Authors = authors ?? new List<AuthorDetails>()
        });
    }

    private static ApiResponse<object> BuildLegacyCacheResponse(bool cacheHit, string cachedTime)
    {
        return ApiResponse<object>.SuccessMessage(new
        {
            CacheType = "System.Runtime.Caching.MemoryCache",
            CacheStatus = cacheHit ? "HIT" : "MISS",
            CachedTimeUtc = cachedTime,
            Note = "This legacy API is useful to recognize in older .NET applications. Prefer IMemoryCache for new ASP.NET Core code."
        });
    }
}
