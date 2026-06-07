using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Repository.AuthorRepository;
using DlmsWebApi.Repository.Models;
using DlmsWebApi.Repository.RepositoryPattern;
using DlmsWebApi.Shared;
using DlmsWebApi.Shared.AuthorData;

namespace DlmsWebApi.Business.AuthorBusiness
{
    public class AuthorBusiness : IAuthorBusiness
    {
        private readonly IAuthorRepository _authorRepository;

        private readonly IRepository<Author> _repository;

        public AuthorBusiness(IAuthorRepository authorRepository, IRepository<Author> repository)
        {
            _authorRepository = authorRepository;
            _repository = repository;
        }

        public async Task<bool> Add(AuthorDetails author)
        {
            var authorEntity = new Author
            {
                FirstName = author.FirstName,
                MiddleName = author.MiddleName,
                LastName = author.LastName,
                Bio = author.Bio,
                DateOfBirth = author.DateOfBirth,
                Status = author.Status,
                CreatedBy = author.User,
                CreatedDate = DateTime.Now
            };
            return await _authorRepository.Add(authorEntity);
        }

        public async Task<bool> Edit(AuthorDetails author)
        {
            return await _authorRepository.Edit(author);
        }

        public async Task<AuthorDetails> GetDetails(int id)
        {
            var authorData = await _authorRepository.GetDetails(id);
            var authorDetails = new AuthorDetails
            {
                AuthorId = authorData.AuthorId,
                FirstName = authorData.FirstName,
                MiddleName = authorData.MiddleName,
                LastName = authorData.LastName,
                Bio = authorData.Bio,
                DateOfBirth = authorData.DateOfBirth,
                Status = authorData.Status
            };
            return authorDetails;
        }

        public async Task<List<AuthorDetails>> GetList()
        {
            List<AuthorDetails> authorList = new List<AuthorDetails>();
            var authors = await _authorRepository.GetList();
            foreach (var author in authors)
            {
                authorList.Add(new AuthorDetails
                {
                    AuthorId = author.AuthorId,
                    AuthorIdString = EncryptionHelper.Encrypt(author.AuthorId.ToString()),
                    FirstName = author.FirstName,
                    MiddleName = author.MiddleName,
                    LastName = author.LastName,
                    Bio = author.Bio,
                    DateOfBirth = author.DateOfBirth,
                    Status = string.IsNullOrEmpty(author.Status) ? "A" : author.Status,
                });
            }
            return authorList;
        }

        

        public async Task<bool> UpdateStatus(int authorId, string user)
        {
            var result = await _authorRepository.UpdateStatus(authorId, user);
            return result;
        }

        public async Task<ApiResponse<PagedResult<AuthorDetails>>> GetListPaginated(PaginationParams pagination, CancellationToken ct)
        {
            // 1) Fetch all authors from repository. Current repository exposes a List<Author>,
            //    so we page in-memory. If the dataset grows, consider adding a repository
            //    method that returns IQueryable or accepts pagination params to perform
            //    database-side paging for performance and reduced memory usage.
            var authors = await _authorRepository.GetList();

            // Honor cancellation request early.
            ct.ThrowIfCancellationRequested();

            // 2) Compute counts and apply ordering + paging using LINQ on the in-memory list.
            var totalCount = authors?.Count ?? 0;

            var pagedAuthors = authors
                .OrderBy(a => a.AuthorId) // always order before paging to ensure deterministic results
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            // 3) Map repository Author entities to AuthorDetails DTOs expected by clients.
            var authorDetailsList = pagedAuthors.Select(author => new AuthorDetails
            {
                AuthorId = author.AuthorId,
                AuthorIdString = EncryptionHelper.Encrypt(author.AuthorId.ToString()),
                FirstName = author.FirstName,
                MiddleName = author.MiddleName,
                LastName = author.LastName,
                Bio = author.Bio,
                DateOfBirth = author.DateOfBirth,
                Status = string.IsNullOrEmpty(author.Status) ? "A" : author.Status,
            }).ToList();

            // 4) Build paged result and wrap in ApiResponse. Use PagedResult<AuthorDetails> so Items
            //    is a simple collection of AuthorDetails and serializes naturally for clients.
            var pagedResult = new PagedResult<AuthorDetails>
            {
                Items = authorDetailsList,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };

            return ApiResponse<PagedResult<AuthorDetails>>.SuccessMessage(pagedResult);
        }



        public async Task<List<AuthorDetails>> GetListRepositoryPattern()
        {
            var authorList = await _repository.GetAllAsync();
            return authorList.Select(author => new AuthorDetails
            {
                AuthorId = author.AuthorId,
                FirstName = author.FirstName,
                MiddleName = author.MiddleName,
                LastName = author.LastName,
                Bio = author.Bio,
                DateOfBirth = author.DateOfBirth,
                Status = string.IsNullOrEmpty(author.Status) ? "A" : author.Status,
            }).ToList();
        }
    }
}
