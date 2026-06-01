using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Repository.AuthorRepository;
using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared.AuthorData;

namespace DlmsWebApi.Business.AuthorBusiness
{
    public class AuthorBusiness : IAuthorBusiness
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorBusiness(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
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
    }
}
