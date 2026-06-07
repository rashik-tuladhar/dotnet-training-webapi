using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared;
using DlmsWebApi.Shared.AuthorData;
using DlmsWebApi.Shared.BookData;

namespace DlmsWebApi.Business.BookBusiness
{
    public interface IBookBusiness
    {
        Task<bool> AddBook(BookDetails book);
        Task<bool> EditBook(BookDetails book);
        Task<BookDetails> GetBookDetails(int id);
        Task<List<BookDetails>> GetList();
        Task<BookDetails> GetDetails(int BookId);
        Task<bool> UpdateStatus(int bookId, string user);
        Task<ApiResponse<PagedResult<BookDetails>>> GetListPaginated(PaginationParams pagination, CancellationToken ct);

    }
}
