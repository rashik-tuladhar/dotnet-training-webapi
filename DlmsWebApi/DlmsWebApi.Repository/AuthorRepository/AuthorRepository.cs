using DlmsWebApi.Repository.Data;
using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared.AuthorData;
using Microsoft.EntityFrameworkCore;

namespace DlmsWebApi.Repository.AuthorRepository
{
    public class AuthorRepository : IAuthorRepository   
    {
        private readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(Author data)
        {
            await _context.Author.AddAsync(data);
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

        public async Task<bool> Edit(AuthorDetails author)
        {
            var details = _context.Author.FirstOrDefault(x => x.AuthorId == author.AuthorId);
            if (details != null)
            {
                details.FirstName = author.FirstName;
                details.MiddleName = author.MiddleName;
                details.LastName = author.LastName;
                details.Bio = author.Bio;
                details.DateOfBirth = author.DateOfBirth;
                details.Status = author.Status;
                details.ModifiedBy = author.User;
                details.ModifiedDate = DateTime.UtcNow;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
            }
            return false;
        }

        public async Task<Author> GetDetails(int id)
        {
            var details = await _context.Author.AsNoTracking().FirstOrDefaultAsync(x => x.AuthorId == id);
            return details;
        }

        public async Task<List<Author>> GetList()
        {
            var listValue = await _context.Author.AsNoTracking().OrderByDescending(x => x.AuthorId).ToListAsync();
            return listValue;
        }

        public async Task<bool> UpdateStatus(int authorId, string user)
        {
            var details = await _context.Author.FirstOrDefaultAsync(x => x.AuthorId == authorId);
            if (details != null)
            {
                details.Status = details.Status == "A" ? "N" : "A";
                details.ModifiedBy = user;
                details.ModifiedDate = DateTime.UtcNow;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
            }
            return false;
        }
    }
}
