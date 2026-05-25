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
    [RegularExpression("^(Active|Inactive|Archived|Pending)$", ErrorMessage = "Status must be 'Active', 'Inactive', 'Archived', or 'Pending'.")]
    public string Status { get; set; } = "Active";

    [Required]
    [RegularExpression("^(B2C|B2B)$", ErrorMessage = "CustomerType must be 'B2C' or 'B2B'.")]
    public string CustomerType { get; set; } = "B2C";

    [MaxLength(200)]
    public string? CompanyName { get; set; }
}
