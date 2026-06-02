using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared.CategoryData;

namespace DlmsWebApi.Repository.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<bool> Add(Category categoryEntity);
        Task<bool> Edit(CategoryDetails category);
        Task<Category> GetDetails(int id);
        Task<List<Category>> GetList();
        Task<bool> UpdateStatus(int categoryId, string user);
    }
}
