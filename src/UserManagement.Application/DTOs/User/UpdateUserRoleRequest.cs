using System.ComponentModel.DataAnnotations;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.DTOs.User;

/// <summary>
/// Request payload to update user role.
/// </summary>
public class UpdateUserRoleRequest
{
    [Required(ErrorMessage = "Role is required.")]
    public UserRole Role { get; set; }
}
