
using MekanikApi.Domain.Entities;

namespace MekanikApi.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetByPhoneNumberAsync(string phoneNumber);
    }
}