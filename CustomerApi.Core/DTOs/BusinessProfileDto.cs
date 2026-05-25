using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Core.DTOs;

public class BusinessProfileDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    public string? LeadSource { get; set; }
    public string LifecycleStage { get; set; } = "Lead";
}

public class CreateOrUpdateBusinessProfileDto
{
    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [MaxLength(100)]
    public string? JobTitle { get; set; }

    [MaxLength(100)]
    public string? LeadSource { get; set; }

    [RegularExpression(@"^(Lead|Opportunity|Customer|Churned)$")]
    public string? LifecycleStage { get; set; }
}
