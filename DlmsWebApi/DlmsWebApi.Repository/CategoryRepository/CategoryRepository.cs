using DlmsWebApi.Repository.Data;
using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared.CategoryData;
using Microsoft.EntityFrameworkCore;
namespace DlmsWebApi.Repository.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(Category data)
        {
            await _context.Category.AddAsync(data);
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                var stackTrace = ex.StackTrace;
                return false;
            }

        }

        public async Task<bool> Edit(CategoryDetails category)
        {
            var details = _context.Category.FirstOrDefault(x => x.CategoryId == category.CategoryId);
            if (details != null)
            {
                details.Name = category.Name;
                details.Description = category.Description;
                details.Status = category.Status;
                details.ModifiedBy = category.User;
                details.ModifiedDate = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
            }
            return false;
        }


        public async Task<Category> GetDetails(int id)
        {
            var details = await _context.Category.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == id);
            return details;
        }


        public async Task<List<Category>> GetList()
        {
            var listValue = await _context.Category.AsNoTracking().OrderByDescending(x => x.CategoryId).ToListAsync();
            return listValue;
        }

        public async Task<bool> UpdateStatus(int categoryId, string user)
        {
            var details = await _context.Category.FirstOrDefaultAsync(x => x.CategoryId == categoryId);
            if (details != null)
            {
                details.Status = details.Status == "A" ? "N" : "A";
                details.ModifiedBy = user;
                details.ModifiedDate = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
            }
            return false;
        }
    }
}
