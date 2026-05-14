using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Core.DTOs;

public class CreateCustomerDto
{
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Company { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(active|inactive)$", ErrorMessage = "Status must be 'active' or 'inactive'.")]
    public string Status { get; set; } = "active";
}
