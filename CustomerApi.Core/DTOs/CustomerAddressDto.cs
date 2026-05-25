using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Core.DTOs;

public class CustomerAddressDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? StateProvince { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class CreateAddressDto
{
    [Required]
    [RegularExpression(@"^(Billing|Shipping|Office)$")]
    public string AddressType { get; set; } = "Billing";

    [Required]
    [MaxLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? AddressLine2 { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? StateProvince { get; set; }

    [Required]
    [MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string CountryCode { get; set; } = string.Empty;

    public bool IsPrimary { get; set; } = false;
}

public class UpdateAddressDto
{
    [RegularExpression(@"^(Billing|Shipping|Office)$")]
    public string? AddressType { get; set; }

    [MaxLength(200)]
    public string? AddressLine1 { get; set; }

    [MaxLength(200)]
    public string? AddressLine2 { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? StateProvince { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(10)]
    public string? CountryCode { get; set; }

    public bool? IsPrimary { get; set; }
}
