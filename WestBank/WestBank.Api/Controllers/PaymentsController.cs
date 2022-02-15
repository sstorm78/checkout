using Microsoft.AspNetCore.Mvc;
using System;
using WestBank.Models;
using WestBank.Services;

namespace WestBank.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentsService _paymentService;

        public PaymentsController(
            IPaymentsService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] PaymentRequest paymentRequest)
        {
            var paymentResponse = _paymentService.Process(paymentRequest);

            if (paymentResponse.Status == PaymentStatus.Declined)
            {
                return BadRequest(paymentResponse.Reason);
            }

            return Created($"http://localhost:60000/payments/{paymentResponse.PaymentId}", paymentResponse.Reason);
        }

        [Route("{paymentId}")]
        [HttpGet]
        public IActionResult Get(Guid paymentId)
        {
            var paymentDetails = _paymentService.GetDetails(paymentId);

            if (paymentDetails == null)
            {
                return NotFound();
            }

            return Ok(paymentDetails);
        }
    }
}
