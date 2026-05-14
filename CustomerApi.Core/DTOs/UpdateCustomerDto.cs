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

    [MaxLength(200)]
    public string? Company { get; set; }

    [RegularExpression("^(active|inactive)$", ErrorMessage = "Status must be 'active' or 'inactive'.")]
    public string? Status { get; set; }
}
