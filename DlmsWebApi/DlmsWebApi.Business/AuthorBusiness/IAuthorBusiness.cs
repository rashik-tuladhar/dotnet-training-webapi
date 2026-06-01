using DlmsWebApi.Shared.AuthorData;

namespace DlmsWebApi.Business.AuthorBusiness
{
    public interface IAuthorBusiness
    {
        Task<bool> Add(AuthorDetails publication);
        Task<bool> Edit(AuthorDetails publication);
        Task<AuthorDetails> GetDetails(int id);
        Task<List<AuthorDetails>> GetList();
        Task<bool> UpdateStatus(int publicationId, string user);
    }
}
