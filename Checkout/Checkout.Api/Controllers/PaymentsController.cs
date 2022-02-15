using Microsoft.AspNetCore.Mvc;
using System;
using Checkout.Models;
using Checkout.Services;
using System.Threading.Tasks;

namespace Checkout.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentProcessingService _paymentProcessingService;

        public PaymentsController(
            IPaymentProcessingService paymentProcessingService)
        {
            _paymentProcessingService = paymentProcessingService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PaymentRequestModel paymentRequest)
        {
            var paymentResponse = await _paymentProcessingService.Process(paymentRequest);

            if (paymentResponse.Status == PaymentStatus.Declined)
            {
                return BadRequest(paymentResponse.Reason);
            }

            return Created($"http://localhost:50000/payments/{paymentResponse.PaymentId}", paymentResponse.Reason);
        }

        [Route("{paymentId}")]
        [HttpGet]
        public async Task<IActionResult> Get(Guid paymentId)
        {
            var paymentDetails = await _paymentProcessingService.GetDetails(paymentId);

            if (paymentDetails == null)
            {
                return NotFound();
            }

            return Ok(paymentDetails);
        }
    }
}
