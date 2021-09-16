using Checkout.Models;
using Checkout.Services;
using Microsoft.AspNetCore.Mvc;
using System;

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
        public IActionResult Post([FromBody]PaymentRequestModel paymentRequest)
        {
            var paymentResponse = _paymentProcessingService.Process(paymentRequest);

            if (paymentResponse.Status == PaymentStatus.Declined)
            {
                return BadRequest(paymentResponse.Reason);
            }

            return Created($"http://localhost:50000/payments/{paymentResponse.PaymentId}", paymentResponse.Reason);
        }

        [Route("{paymentId}")]
        [HttpGet]
        public IActionResult Get(Guid paymentId)
        {
            var paymentDetails = _paymentProcessingService.GetDetails(paymentId);

            if (paymentDetails == null)
            {
                return new NotFoundResult();
            }

            return Ok(paymentDetails);
        }
    }
}
