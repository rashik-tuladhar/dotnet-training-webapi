namespace DlmsWebApi.Caching;

public interface IAuthorCacheInvalidator
{
    Task ClearAsync(CancellationToken cancellationToken = default);
}
