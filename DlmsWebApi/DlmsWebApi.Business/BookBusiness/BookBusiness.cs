using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Repository.BookRepository;
using DlmsWebApi.Repository.Models;
using DlmsWebApi.Repository.RepositoryPattern;
using DlmsWebApi.Shared;
using DlmsWebApi.Shared.AuthorData;
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

        public async Task<ApiResponse<PagedResult<BookDetails>>> GetListPaginated(PaginationParams pagination, CancellationToken ct)
        {
            // 1) Fetch all books from repository. Current repository exposes a List<Author>,
            //    so we page in-memory. If the dataset grows, consider adding a repository
            //    method that returns IQueryable or accepts pagination params to perform
            //    database-side paging for performance and reduced memory usage.
            var books = await _Bookrepository.GetList();

            // Honor cancellation request early.
            ct.ThrowIfCancellationRequested();

            // 2) Compute counts and apply ordering + paging using LINQ on the in-memory list.
            var totalCount = books?.Count ?? 0;

            var pagedAuthors = books
                .OrderBy(a => a.BookId) // always order before paging to ensure deterministic results
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            // 3) Map repository Author entities to BookDetails DTOs expected by clients.
            var bookDetailsList = pagedAuthors.Select(book => new BookDetails
            {
                BookId = book.BookId,
                BookIdString = EncryptionHelper.Encrypt(book.BookId.ToString()),

                Name = book.Name,
                Author = book.Author,
                Publication = book.Publication,
                Status = string.IsNullOrEmpty(book.Status) ? "A" : book.Status,
                ImageUrl = book.ImageUrl
            }).ToList();

            // 4) Build paged result and wrap in ApiResponse. Use PagedResult<BookDetails> so Items
            //    is a simple collection of BookDetails and serializes naturally for clients.
            var pagedResult = new PagedResult<BookDetails>
            {
                Items = bookDetailsList,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };

            return ApiResponse<PagedResult<BookDetails>>.SuccessMessage(pagedResult);
        }
        public async Task<List<BookDetails>> GetListRepositoryPattern()
        {
            var bookList = await _repository.GetAllAsync();
            return bookList.Select(book => new BookDetails
            {
                BookId = book.BookId,

                Name = book.Name,
                Author = book.Author,
                Publication = book.Publication,
                Status = string.IsNullOrEmpty(book.Status) ? "A" : book.Status,
                ImageUrl = book.ImageUrl
            }).ToList();
        }
    }
}
