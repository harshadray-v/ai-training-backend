using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerApi.Core.Entities;

public class CustomerBusinessProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [MaxLength(100)]
    public string? JobTitle { get; set; }

    [MaxLength(100)]
    public string? LeadSource { get; set; }

    [Required]
    [MaxLength(20)]
    public string LifecycleStage { get; set; } = "Lead"; // Lead, Opportunity, Customer, Churned

    // Navigation property
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;
}
