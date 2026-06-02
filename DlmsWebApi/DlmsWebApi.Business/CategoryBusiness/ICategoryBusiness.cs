using DlmsWebApi.Shared.CategoryData;

namespace DlmsWebApi.Business.CategoryBusiness
{
    public interface ICategoryBusiness
    {
        Task<bool> Add(CategoryDetails category);
        Task<bool> Edit(CategoryDetails category);
        Task<CategoryDetails> GetDetails(int id);
        Task<List<CategoryDetails>> GetList();
        Task<bool> UpdateStatus(int categoryId, string user);
    }
}
