using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MekanikApi.Application.DTOs.Roles;
using MekanikApi.Application.Interfaces;

namespace MekanikApi.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public RoleService(IMapper mapper, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<RoleRequest> CreateRole(RoleRequest model)
        {
            var role = new IdentityRole<Guid> { Name = model.Name.ToLower() };
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                var roleDto = _mapper.Map<RoleRequest>(role);
                return roleDto;
            }
            else
            {
                throw new Exception("Role creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<RoleRequest> UpdateRole(Guid roleId, RoleRequest model)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString()) ?? throw new ApplicationException("Role not found");
            role.Name = model.Name.ToLower();

            var updatedRole = await _roleManager.UpdateAsync(role);
            var roleDto = _mapper.Map<RoleRequest>(updatedRole);
            return roleDto;
        }

        public async Task DeleteRole(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString()) ?? throw new ApplicationException("Role not found");
            await _roleManager.DeleteAsync(role);
        }

        public async Task<IEnumerable<RoleRequest>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleDtos = _mapper.Map<IEnumerable<RoleRequest>>(roles);
            return roleDtos;
        }

        public async Task<RoleRequest> GetRoleById(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString()) ?? throw new ApplicationException("Role not found");
            var roleDto = _mapper.Map<RoleRequest>(role);
            return roleDto;
        }
    }
}