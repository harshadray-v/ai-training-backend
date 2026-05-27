using CustomerApi.Core.Configuration;
using CustomerApi.Core.DTOs;
using CustomerApi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CustomerApi.Api.Controllers;

/// <summary>
/// Payment endpoints powered by Stripe.
///
/// Integration Flow:
/// 1. Frontend calls GET /api/payments/config to get the publishable key
/// 2. Frontend calls POST /api/payments/intents to create a PaymentIntent
/// 3. Frontend receives clientSecret and uses Stripe.js to collect card details (PCI compliant)
/// 4. Stripe.js confirms the payment — card data never touches our server
/// 5. Frontend polls GET /api/payments/intents/{id} or uses Stripe webhooks to verify status
///
/// Security Considerations:
/// - Secret API key stored in environment variables / user-secrets (never committed)
/// - Publishable key is safe to expose to frontend
/// - Card details handled entirely by Stripe.js (PCI DSS Level 1 compliant)
/// - All requests have configurable timeouts and automatic retries
/// - Error messages are sanitized before returning to clients
/// - Payment intent IDs validated before processing
/// </summary>
[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Get the Stripe publishable key for frontend Stripe.js initialization.
    /// This key is safe to expose publicly.
    /// </summary>
    /// <remarks>
    /// GET /api/payments/config
    ///
    /// Response 200:
    /// { "publishableKey": "pk_test_xxxx" }
    /// </remarks>
    [HttpGet("config")]
    [ProducesResponseType(200)]
    public IActionResult GetConfig([FromServices] IOptions<StripeSettings> settings)
    {
        return Ok(new { publishableKey = settings.Value.PublishableKey });
    }

    /// <summary>
    /// Create a PaymentIntent. Returns a client_secret for frontend payment confirmation.
    /// </summary>
    /// <remarks>
    /// POST /api/payments/intents
    ///
    /// Request Body:
    /// {
    ///   "amount": 5000,            // Required. Amount in cents ($50.00)
    ///   "currency": "usd",         // Optional. Defaults to configured currency
    ///   "customerId": 1,           // Required. Internal customer ID
    ///   "description": "Order #1", // Optional.
    ///   "metadata": {}             // Optional. Additional key-value pairs
    /// }
    ///
    /// Response 201:
    /// {
    ///   "id": "pi_3xyz...",
    ///   "amount": 5000,
    ///   "currency": "usd",
    ///   "status": "requires_payment_method",
    ///   "clientSecret": "pi_3xyz..._secret_abc",
    ///   "description": "Order #1",
    ///   "created": "2024-01-15T10:30:00Z"
    /// }
    ///
    /// Response 400: Validation errors
    /// Response 502: Stripe API error
    /// </remarks>
    [HttpPost("intents")]
    [ProducesResponseType(typeof(PaymentIntentResultDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(502)]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentDto dto)
    {
        var result = await _paymentService.CreatePaymentIntentAsync(dto);

        if (result.Status == "error")
        {
            _logger.LogWarning("Payment intent creation failed: {Error}", result.ErrorMessage);
            return StatusCode(502, new { error = result.ErrorMessage });
        }

        return CreatedAtAction(nameof(GetPaymentIntent), new { intentId = result.Id }, result);
    }

    /// <summary>
    /// Retrieve a PaymentIntent by its Stripe ID.
    /// </summary>
    /// <remarks>
    /// GET /api/payments/intents/{intentId}
    ///
    /// Response 200:
    /// {
    ///   "id": "pi_3xyz...",
    ///   "amount": 5000,
    ///   "currency": "usd",
    ///   "status": "succeeded",
    ///   "created": "2024-01-15T10:30:00Z"
    /// }
    ///
    /// Response 400: Invalid intent ID format
    /// Response 502: Stripe API error
    /// </remarks>
    [HttpGet("intents/{intentId}")]
    [ProducesResponseType(typeof(PaymentIntentResultDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(502)]
    public async Task<IActionResult> GetPaymentIntent(string intentId)
    {
        if (string.IsNullOrWhiteSpace(intentId) || !intentId.StartsWith("pi_"))
            return BadRequest(new { error = "Invalid payment intent ID format. Must start with 'pi_'." });

        var result = await _paymentService.GetPaymentIntentAsync(intentId);

        if (result.Status == "error")
            return StatusCode(502, new { error = result.ErrorMessage });

        return Ok(result);
    }

    /// <summary>
    /// Cancel a PaymentIntent that has not yet been captured.
    /// </summary>
    /// <remarks>
    /// POST /api/payments/intents/{intentId}/cancel
    ///
    /// Response 200:
    /// { "id": "pi_3xyz...", "status": "canceled", ... }
    ///
    /// Response 400: Invalid intent ID
    /// Response 502: Stripe API error
    /// </remarks>
    [HttpPost("intents/{intentId}/cancel")]
    [ProducesResponseType(typeof(PaymentIntentResultDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(502)]
    public async Task<IActionResult> CancelPaymentIntent(string intentId)
    {
        if (string.IsNullOrWhiteSpace(intentId) || !intentId.StartsWith("pi_"))
            return BadRequest(new { error = "Invalid payment intent ID format. Must start with 'pi_'." });

        var result = await _paymentService.CancelPaymentIntentAsync(intentId);

        if (result.Status == "error")
            return StatusCode(502, new { error = result.ErrorMessage });

        return Ok(result);
    }
}
