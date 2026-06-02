using DlmsWebApi.Shared.BookData;

namespace DlmsWebApi.Business.BookBusiness
{
    public interface IBookBusiness
    {
        Task<bool> AddBook(BookDetails book);
        Task<bool> EditBooks(BookDetails book);
        Task<BookDetails> GetBookDetails(int id);
        Task<List<BookDetails>> GetBookList();
        Task<bool> UpdateStatus(int bookId, string user);
    }
}
