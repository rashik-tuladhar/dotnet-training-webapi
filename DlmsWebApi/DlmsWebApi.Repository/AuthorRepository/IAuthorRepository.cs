using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared.AuthorData;

namespace DlmsWebApi.Repository.AuthorRepository
{
    public interface IAuthorRepository
    {
        Task<bool> Add(Author authorEntity);
        Task<bool> Edit(AuthorDetails author);  
        Task<Author> GetDetails(int id);
        Task<List<Author>> GetList();
        Task<bool> UpdateStatus(int authorId, string user);
    }
}
