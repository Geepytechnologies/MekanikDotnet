using Microsoft.EntityFrameworkCore;
using MekanikApi.Domain.Interfaces;
using MekanikApi.Infrastructure.DataContext;

namespace MekanikApi.Domain.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbcontext;

        public GenericRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbcontext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbcontext.Set<T>().FindAsync(id);
        }

        public void Add(T entity)
        {
            _dbcontext.Set<T>().Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _dbcontext.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbcontext.Entry(entity).State = EntityState.Modified;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbcontext.Entry(entity).State = EntityState.Modified;
            await _dbcontext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbcontext.Set<T>().Remove(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _dbcontext.Set<T>().Remove(entity);
            await _dbcontext.SaveChangesAsync();
        }
    }
}