using CustomerApi.Core.DTOs;

namespace CustomerApi.Core.Services;

/// <summary>
/// Payment service abstraction for testability.
/// Implementations should handle retries, timeouts, and error mapping.
/// </summary>
public interface IPaymentService
{
    /// <summary>Create a new PaymentIntent for the given customer/amount.</summary>
    Task<PaymentIntentResultDto> CreatePaymentIntentAsync(CreatePaymentIntentDto request);

    /// <summary>Retrieve an existing PaymentIntent by its Stripe ID.</summary>
    Task<PaymentIntentResultDto> GetPaymentIntentAsync(string paymentIntentId);

    /// <summary>Cancel a PaymentIntent that has not been captured.</summary>
    Task<PaymentIntentResultDto> CancelPaymentIntentAsync(string paymentIntentId);

    /// <summary>List recent payments for a Stripe customer.</summary>
    Task<IEnumerable<PaymentIntentResultDto>> GetPaymentHistoryAsync(string stripeCustomerId);
}
