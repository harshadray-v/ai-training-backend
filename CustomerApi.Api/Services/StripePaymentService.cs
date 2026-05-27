using CustomerApi.Core.Configuration;
using CustomerApi.Core.DTOs;
using CustomerApi.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace CustomerApi.Api.Services;

/// <summary>
/// Stripe-based payment service with:
/// - Configurable retry logic (via Stripe SDK MaxNetworkRetries)
/// - Request timeouts via CancellationTokenSource
/// - Sanitized error messages (no internal Stripe details leaked to client)
/// - Structured logging for audit/traceability
/// </summary>
public class StripePaymentService : IPaymentService
{
    private readonly StripeSettings _settings;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(IOptions<StripeSettings> settings, ILogger<StripePaymentService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        // Configure Stripe SDK — key from environment/user-secrets, never hardcoded
        StripeConfiguration.ApiKey = _settings.SecretKey;
        StripeConfiguration.MaxNetworkRetries = _settings.MaxRetryAttempts;
    }

    public async Task<PaymentIntentResultDto> CreatePaymentIntentAsync(CreatePaymentIntentDto request)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = request.Amount,
                Currency = request.Currency ?? _settings.Currency,
                Description = request.Description,
                Metadata = request.Metadata ?? new Dictionary<string, string>(),
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            // Store internal customer ID in metadata for traceability
            options.Metadata["internal_customer_id"] = request.CustomerId.ToString();

            var service = new PaymentIntentService();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_settings.TimeoutSeconds));
            var intent = await service.CreateAsync(options, cancellationToken: cts.Token);

            _logger.LogInformation(
                "PaymentIntent {IntentId} created — customer={CustomerId}, amount={Amount}, currency={Currency}",
                intent.Id, request.CustomerId, request.Amount, intent.Currency);

            return MapToDto(intent);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex,
                "Stripe error creating PaymentIntent for customer {CustomerId}: {Code} — {Message}",
                request.CustomerId, ex.StripeError?.Code, ex.StripeError?.Message);

            return new PaymentIntentResultDto
            {
                Status = "error",
                ErrorMessage = SanitizeError(ex)
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("PaymentIntent creation timed out for customer {CustomerId}", request.CustomerId);
            return new PaymentIntentResultDto
            {
                Status = "error",
                ErrorMessage = "Request timed out. Please try again."
            };
        }
    }

    public async Task<PaymentIntentResultDto> GetPaymentIntentAsync(string paymentIntentId)
    {
        try
        {
            var service = new PaymentIntentService();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_settings.TimeoutSeconds));
            var intent = await service.GetAsync(paymentIntentId, cancellationToken: cts.Token);
            return MapToDto(intent);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error retrieving PaymentIntent {IntentId}", paymentIntentId);
            return new PaymentIntentResultDto { Status = "error", ErrorMessage = SanitizeError(ex) };
        }
        catch (OperationCanceledException)
        {
            return new PaymentIntentResultDto { Status = "error", ErrorMessage = "Request timed out." };
        }
    }

    public async Task<PaymentIntentResultDto> CancelPaymentIntentAsync(string paymentIntentId)
    {
        try
        {
            var service = new PaymentIntentService();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_settings.TimeoutSeconds));
            var intent = await service.CancelAsync(paymentIntentId, cancellationToken: cts.Token);

            _logger.LogInformation("PaymentIntent {IntentId} cancelled", paymentIntentId);
            return MapToDto(intent);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error cancelling PaymentIntent {IntentId}", paymentIntentId);
            return new PaymentIntentResultDto { Status = "error", ErrorMessage = SanitizeError(ex) };
        }
        catch (OperationCanceledException)
        {
            return new PaymentIntentResultDto { Status = "error", ErrorMessage = "Request timed out." };
        }
    }

    public async Task<IEnumerable<PaymentIntentResultDto>> GetPaymentHistoryAsync(string stripeCustomerId)
    {
        try
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentListOptions
            {
                Customer = stripeCustomerId,
                Limit = 20
            };

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_settings.TimeoutSeconds));
            var intents = await service.ListAsync(options, cancellationToken: cts.Token);

            return intents.Data.Select(MapToDto);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error fetching history for customer {StripeCustomerId}", stripeCustomerId);
            return Enumerable.Empty<PaymentIntentResultDto>();
        }
        catch (OperationCanceledException)
        {
            return Enumerable.Empty<PaymentIntentResultDto>();
        }
    }

    // ----- Private Helpers -----

    private static PaymentIntentResultDto MapToDto(PaymentIntent intent) => new()
    {
        Id = intent.Id,
        Amount = intent.Amount,
        Currency = intent.Currency,
        Status = intent.Status,
        ClientSecret = intent.ClientSecret,
        Description = intent.Description,
        Created = intent.Created
    };

    /// <summary>
    /// Sanitize Stripe errors to prevent leaking internal API details to the client.
    /// Only card_error and validation_error messages are passed through directly.
    /// </summary>
    private static string SanitizeError(StripeException ex)
    {
        return ex.StripeError?.Type switch
        {
            "card_error" => ex.StripeError.Message,
            "validation_error" => ex.StripeError.Message,
            "invalid_request_error" => "Invalid payment request. Please check your input.",
            "authentication_error" => "Payment service configuration error. Contact support.",
            "rate_limit_error" => "Too many requests. Please try again shortly.",
            _ => "An error occurred processing your payment. Please try again."
        };
    }
}
