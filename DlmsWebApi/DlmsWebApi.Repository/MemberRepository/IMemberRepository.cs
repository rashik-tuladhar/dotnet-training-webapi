using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared.MemberData;

namespace DlmsWebApi.Repository.MemberRepository
{
    public interface IMemberRepository
    {
        Task<bool> Add(Member memberEntity);
        Task<bool> Edit(MemberDetails member);
        Task<Member> GetDetails(int id);
        Task<List<Member>> GetList();
        Task<bool> UpdateStatus(int bookId, string user);
    }
}
