using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Core.DTOs;

public class UpdateCustomerDto
{
    [MinLength(2)]
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MinLength(2)]
    [MaxLength(100)]
    public string? LastName { get; set; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [RegularExpression("^(Active|Inactive|Archived|Pending)$", ErrorMessage = "Status must be 'Active', 'Inactive', 'Archived', or 'Pending'.")]
    public string? Status { get; set; }

    [RegularExpression("^(B2C|B2B)$", ErrorMessage = "CustomerType must be 'B2C' or 'B2B'.")]
    public string? CustomerType { get; set; }

    [MaxLength(200)]
    public string? CompanyName { get; set; }
}
