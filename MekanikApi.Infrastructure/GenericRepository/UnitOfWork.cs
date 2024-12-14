using MekanikApi.Domain.Interfaces;
using MekanikApi.Infrastructure.DataContext;

namespace MekanikApi.Domain.GenericRepository
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.TryGetValue(typeof(T), out var repo))
            {
                return (IGenericRepository<T>)repo;
            }

            var repositoryInstance = new GenericRepository<T>(_dbContext);
            _repositories[typeof(T)] = repositoryInstance;
            return repositoryInstance;
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}