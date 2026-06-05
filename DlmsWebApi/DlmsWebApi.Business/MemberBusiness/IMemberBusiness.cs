using DlmsWebApi.Shared.MemberData;

namespace DlmsWebApi.Business.MemberBusiness
{
    public interface IMemberBusiness
    {
        Task<bool> Add(MemberDetails publication);
        Task<bool> Edit(MemberDetails publication);
        Task<MemberDetails> GetDetails(int id);
        Task<List<MemberDetails>> GetList();
        Task<bool> UpdateStatus(int publicationId, string user);
    }
}
