using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerApi.Core.Entities;

public class CustomerAddress
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(20)]
    public string AddressType { get; set; } = "Billing"; // Billing, Shipping, Office

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

    // Navigation property
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;
}
