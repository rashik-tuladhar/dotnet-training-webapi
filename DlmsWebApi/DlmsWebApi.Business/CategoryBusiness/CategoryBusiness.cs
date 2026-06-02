using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Repository.CategoryRepository;
using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared.CategoryData;

namespace DlmsWebApi.Business.CategoryBusiness
{
    public class CategoryBusiness : ICategoryBusiness
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryBusiness(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> Add(CategoryDetails category)
        {
            var categoryEntity = new Category
            {
                Name = category.Name,
                Description = category.Description,
                CreatedBy = category.User,
                CreatedDate = DateTime.Now
            };
            return await _categoryRepository.Add(categoryEntity);
        }

        public async Task<bool> Edit(CategoryDetails category)
        {
            return await _categoryRepository.Edit(category);
        }

        public async Task<CategoryDetails> GetDetails(int id)
        {
            var categoryData = await _categoryRepository.GetDetails(id);
            var categoryDetails = new CategoryDetails
            {
                CategoryId = categoryData.CategoryId,
                Name = categoryData.Name,
                Description = categoryData.Description,
                Status = categoryData.Status
            };
            return categoryDetails;
        }

        public async Task<List<CategoryDetails>> GetList()
        {
            List<CategoryDetails> categoryList = new List<CategoryDetails>();
            var categorys = await _categoryRepository.GetList();
            foreach (var category in categorys)
            {
                categoryList.Add(new CategoryDetails
                {
                    CategoryId = category.CategoryId,
                    CategoryIdString = EncryptionHelper.Encrypt(category.CategoryId.ToString()),
                    Name = category.Name,
                    Description = category.Description,
                    Status = string.IsNullOrEmpty(category.Status) ? "A" : category.Status,
                });
            }
            return categoryList;
        }

        public async Task<bool> UpdateStatus(int categoryId, string user)
        {
            var result = await _categoryRepository.UpdateStatus(categoryId, user);
            return result;
        }
    }
}
