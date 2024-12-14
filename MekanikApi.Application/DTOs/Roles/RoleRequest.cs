using System.ComponentModel.DataAnnotations;

namespace MekanikApi.Application.DTOs.Roles
{
    public class RoleRequest
    {

        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}