
using MekanikApi.Domain.Entities;

namespace MekanikApi.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
    }
}