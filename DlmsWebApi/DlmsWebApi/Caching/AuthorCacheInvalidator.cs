using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace DlmsWebApi.Caching;

public class AuthorCacheInvalidator : IAuthorCacheInvalidator
{
    private readonly IDistributedCache _distributedCache;
    private readonly IMemoryCache _memoryCache;
    private readonly IOutputCacheStore _outputCacheStore;

    public AuthorCacheInvalidator(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        IOutputCacheStore outputCacheStore)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _outputCacheStore = outputCacheStore;
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        foreach (var key in AuthorCacheKeys.MemoryKeys)
        {
            _memoryCache.Remove(key);
        }

        foreach (var key in AuthorCacheKeys.DistributedKeys)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }

        await _outputCacheStore.EvictByTagAsync(AuthorCacheKeys.AuthorTag, cancellationToken);
    }
}
