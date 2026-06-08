namespace DlmsWebApi.Caching;

public static class AuthorCacheKeys
{
    public const string AuthorTag = "authors";
    public const string AuthorListMemory = "authors:list:memory";
    public const string AuthorListDistributed = "authors:list:distributed";
    public const string AuthorListHybrid = "authors:list:hybrid";
    public const string AuthorListOutputCachePolicy = "AuthorListOutputCachePolicy";
    public const string AuthorDetailsOutputCachePolicy = "AuthorDetailsOutputCachePolicy";

    public static readonly string[] MemoryKeys =
    [
        AuthorListMemory,
        AuthorListHybrid
    ];

    public static readonly string[] DistributedKeys =
    [
        AuthorListDistributed,
        AuthorListHybrid
    ];
}
