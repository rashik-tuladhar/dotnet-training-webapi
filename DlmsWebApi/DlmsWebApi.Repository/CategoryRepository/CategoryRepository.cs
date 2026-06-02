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
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Edit(CategoryDetails category)
        {
            var details = await _context.Category.FirstOrDefaultAsync(x => x.CategoryId == category.CategoryId);
            if (details == null)
            {
                return false;
            }

            details.Name = category.Name;
            details.Description = category.Description;
            details.Status = category.Status;
            details.ModifiedBy = category.User;
            details.ModifiedDate = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<Category?> GetDetails(int id)
        {
            return await _context.Category.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == id);
        }

        public async Task<List<Category>> GetList()
        {
            return await _context.Category.AsNoTracking().OrderByDescending(x => x.CategoryId).ToListAsync();
        }

        public async Task<bool> UpdateStatus(int categoryId, string user)
        {
            var details = await _context.Category.FirstOrDefaultAsync(x => x.CategoryId == categoryId);
            if (details == null)
            {
                return false;
            }

            details.Status = details.Status == "A" ? "N" : "A";
            details.ModifiedBy = user;
            details.ModifiedDate = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
