using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Repository.BookRepository;
using DlmsWebApi.Repository.Models;
using DlmsWebApi.Repository.RepositoryPattern;
using DlmsWebApi.Shared.BookData;


namespace DlmsWebApi.Business.BookBusiness
{
    public class BookBusiness : IBookBusiness
    {
        private readonly IBookRepository _Bookrepository;
        private readonly IRepository<Book> _repository;

        public BookBusiness(IBookRepository bookRepository,IRepository<Book> repository)
        {
            _Bookrepository = bookRepository;
            _repository = repository;
        }

        public async Task<bool> AddBook(BookDetails book)
        {
            var bookEntity = new Repository.Models.Book
            {
                Name = book.Name,
                Author = book.Author,
                Publication = book.Publication,
                Category = book.Category,
                Isbn = book.Isbn,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies,
                Edition = book.Edition,
                CreatedBy = book.User,
                Status = book.Status,
                ImageUrl = book.ImageUrl
            };
            return await _Bookrepository.AddBook(bookEntity);
        }

        public async Task<bool> EditBook(BookDetails book)
        {
            return await _Bookrepository.EditBook(book);
        }

       

        public async Task<BookDetails> GetBookDetails(int id)
        {
            var bookData = await _Bookrepository.GetBookDetails(id);
            var bookDetails =  new BookDetails
            {
                BookId = bookData.BookId,
                Name = bookData.Name,
                Author = bookData.Author,
                Publication = bookData.Publication,
                Category = bookData.Category,
                Isbn = bookData.Isbn,
                TotalCopies = bookData.TotalCopies,
                AvailableCopies = bookData.AvailableCopies,
                Edition = bookData.Edition,
                ImageUrl = bookData.ImageUrl
            };
            return bookDetails;
        }

        public async Task<List<BookDetails>> GetList()
        {
            List<BookDetails> bookList = new List<BookDetails>();
            var books = await _Bookrepository.GetList();
            foreach (var book in books)
            {
                bookList.Add(new BookDetails
                {
                    BookId = book.BookId,
                    BookIdString = EncryptionHelper.Encrypt(book.BookId.ToString()),

                    Name = book.Name,
                    Author = book.Author,
                    Publication = book.Publication,
                    Status = string.IsNullOrEmpty(book.Status) ? "A" : book.Status,
                    ImageUrl = book.ImageUrl
                });
            }

            //var bookDetail = books.Select(x => new BookDetails
            //{
            //    Name = x.Name,
            //    Author = x.Author,
            //    Publication = x.Publication
            //}).ToList();

            return bookList;
        }

        public Task<BookDetails> GetDetails(int BookId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateStatus(int bookId, string user)
        {
            var result = await _Bookrepository.UpdateStatus(bookId, user);
            return result;
        }
    }
}
