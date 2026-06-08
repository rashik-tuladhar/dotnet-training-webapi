using DlmsWebApi.Shared.PublicationData;

namespace DlmsWebApi.Business.PublicationBusiness
{
    public interface IPublicationBusiness
    {
        Task<bool> Add(PublicationDetails publication);
        Task<bool> Edit(PublicationDetails publication);
        Task<PublicationDetails> GetDetails(int id);
        Task<List<PublicationDetails>> GetList();
        Task<bool> UpdateStatus(int publicationId, string user);
    }
}
