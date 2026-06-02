using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared.BookData;

namespace DlmsWebApi.Repository.BookRepository
{
    public interface IBookRepository
    {
        Task<bool> AddBook(Book book);
        Task<bool> EditBooks(BookDetails book);
        Task<Book> GetBookDetails(int id);
        Task<List<Book>> GetBookList();
        Task<bool> UpdateStatus(int bookId, string user);
    }
}
