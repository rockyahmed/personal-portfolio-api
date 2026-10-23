using System.ComponentModel.DataAnnotations;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.DTOs.User;

/// <summary>
/// Request payload to update account status.
/// </summary>
public class UpdateUserStatusRequest
{
    [Required(ErrorMessage = "Status is required.")]
    public UserStatus Status { get; set; }
}
