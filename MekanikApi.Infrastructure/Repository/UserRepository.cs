using Microsoft.EntityFrameworkCore;
using MekanikApi.Domain.Entities;
using MekanikApi.Domain.GenericRepository;
using MekanikApi.Domain.Interfaces;
using MekanikApi.Infrastructure.DataContext;

namespace MekanikApi.Infrastructure.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _dbcontext;

        public UserRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<User> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbcontext.Users.SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }
    }
}