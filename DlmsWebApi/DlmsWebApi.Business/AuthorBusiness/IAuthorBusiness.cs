using DlmsWebApi.Shared;
using DlmsWebApi.Shared.AuthorData;

namespace DlmsWebApi.Business.AuthorBusiness
{
    public interface IAuthorBusiness
    {
        Task<bool> Add(AuthorDetails publication);
        Task<bool> Edit(AuthorDetails publication);
        Task<AuthorDetails> GetDetails(int id);
        Task<List<AuthorDetails>> GetList();
        Task<List<AuthorDetails>> GetListRepositoryPattern();
        Task<bool> UpdateStatus(int publicationId, string user);
        // Return a paged set of AuthorDetails. Use PagedResult<AuthorDetails> so Items is a collection
        // of AuthorDetails (not a collection-of-collections). This simplifies JSON output for clients.
        Task<ApiResponse<PagedResult<AuthorDetails>>> GetListPaginated(PaginationParams pagination, CancellationToken ct);
    }
}
