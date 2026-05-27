using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Core.DTOs;

/// <summary>
/// Request to create a Stripe PaymentIntent.
/// Amount is in smallest currency unit (e.g., cents for USD).
/// </summary>
public class CreatePaymentIntentDto
{
    /// <summary>Amount in smallest currency unit (e.g., 5000 = $50.00 USD).</summary>
    [Required]
    [Range(50, 99999999, ErrorMessage = "Amount must be between 50 and 99999999 cents")]
    public long Amount { get; set; }

    /// <summary>Three-letter ISO currency code (e.g., "usd").</summary>
    [RegularExpression(@"^[a-z]{3}$", ErrorMessage = "Currency must be a 3-letter lowercase ISO code")]
    public string? Currency { get; set; }

    /// <summary>Internal customer ID to associate with payment.</summary>
    [Required]
    public int CustomerId { get; set; }

    /// <summary>Optional description for the payment.</summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>Optional metadata key-value pairs.</summary>
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Response representing a Stripe PaymentIntent result.
/// </summary>
public class PaymentIntentResultDto
{
    public string Id { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ClientSecret { get; set; }
    public string? Description { get; set; }
    public DateTime Created { get; set; }
    public string? ErrorMessage { get; set; }
}
