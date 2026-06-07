using DlmsWebApi.Repository.Data;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace DlmsWebApi.Repository.RepositoryPattern;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _set;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() => await _set.AsNoTracking().ToListAsync();
    public async Task AddAsync(T entity) => await _set.AddAsync(entity);
    public void Update(T entity) => _set.Update(entity);
    public void Remove(T entity) => _set.Remove(entity);
    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}