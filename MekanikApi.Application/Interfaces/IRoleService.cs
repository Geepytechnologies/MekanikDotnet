using MekanikApi.Application.DTOs.Roles;

namespace MekanikApi.Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleRequest> CreateRole(RoleRequest request);

        Task<RoleRequest> UpdateRole(Guid roleId, RoleRequest model);

        Task DeleteRole(Guid roleId);

        Task<IEnumerable<RoleRequest>> GetRolesAsync();

        Task<RoleRequest> GetRoleById(Guid roleId);
    }
}