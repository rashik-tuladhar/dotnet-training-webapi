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
                Status = string.IsNullOrWhiteSpace(category.Status) ? "A" : category.Status,
                CreatedBy = category.User,
                CreatedDate = DateTime.Now
            };

            return await _categoryRepository.Add(categoryEntity);
        }

        public async Task<bool> Edit(CategoryDetails category)
        {
            return await _categoryRepository.Edit(category);
        }

        public async Task<CategoryDetails?> GetDetails(int id)
        {
            var categoryData = await _categoryRepository.GetDetails(id);
            if (categoryData == null)
            {
                return null;
            }

            return new CategoryDetails
            {
                CategoryId = categoryData.CategoryId,
                CategoryIdString = EncryptionHelper.Encrypt(categoryData.CategoryId.ToString()),
                Name = categoryData.Name,
                Description = categoryData.Description,
                Status = categoryData.Status
            };
        }

        public async Task<List<CategoryDetails>> GetList()
        {
            var categoryList = new List<CategoryDetails>();
            var categories = await _categoryRepository.GetList();

            foreach (var category in categories)
            {
                categoryList.Add(new CategoryDetails
                {
                    CategoryId = category.CategoryId,
                    CategoryIdString = EncryptionHelper.Encrypt(category.CategoryId.ToString()),
                    Name = category.Name,
                    Description = category.Description,
                    Status = string.IsNullOrEmpty(category.Status) ? "A" : category.Status
                });
            }

            return categoryList;
        }

        public async Task<bool> UpdateStatus(int categoryId, string user)
        {
            return await _categoryRepository.UpdateStatus(categoryId, user);
        }
    }
}
