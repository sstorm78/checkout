using System;
using System.Net;
using Checkout.Api.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Checkout.Services;
using Checkout.Models;
using System.Threading.Tasks;

namespace Checkout.Api.Tests.Controllers
{
    [TestFixture]
    public class PaymentsControllerTests
    {
        private Mock<IPaymentProcessingService> _mockPaymentProcessingService;

        [SetUp]
        public void Setup()
        {
            _mockPaymentProcessingService = new Mock<IPaymentProcessingService>();
        }


        [Test]
        public async Task Post_Should_Return_Created_Result_Without_Reason()
        {
            var request = new PaymentRequestModel();

            var response = new PaymentResponseModel
                           {
                               PaymentId = Guid.NewGuid(),
                               Status= "Success"
                           };

            _mockPaymentProcessingService.Setup(i => i.Process(It.IsAny<PaymentRequestModel>()))
                .ReturnsAsync(response);

            var controller = new PaymentsController(_mockPaymentProcessingService.Object);

            var result = await controller.Post(request);

            ((CreatedResult)result).StatusCode.Should().Be((int)HttpStatusCode.Created);
            ((CreatedResult)result).Location.Should().Be($"http://localhost:50000/payments/{response.PaymentId}");
            var reason = ((CreatedResult)result).Value as string;

            reason.Should().BeNull();
        }

        [Test]
        public async Task Post_Should_Return_Created_Result_With_Reason()
        {
            var request = new PaymentRequestModel();

            var response = new PaymentResponseModel
                           {
                               PaymentId = Guid.NewGuid(),
                               Status = "SuccessWithWarning",
                               Reason = "test"
            };

            _mockPaymentProcessingService.Setup(i => i.Process(It.IsAny<PaymentRequestModel>()))
                .ReturnsAsync(response);

            var controller = new PaymentsController(_mockPaymentProcessingService.Object);

            var result = await controller.Post(request);

            ((CreatedResult)result).StatusCode.Should().Be((int)HttpStatusCode.Created);
            ((CreatedResult)result).Location.Should().Be($"http://localhost:50000/payments/{response.PaymentId}");
            var reason = ((CreatedResult)result).Value as string;

            reason.Should().Be("test");
        }

        [Test]
        public async Task Post_Should_Return_BadRequest_Result_With_Reason_If_Payment_Declined()
        {
            var request = new PaymentRequestModel();

            var response = new PaymentResponseModel
                           {
                               Status = "Declined",
                               Reason = "test"
                           };

            _mockPaymentProcessingService.Setup(i => i.Process(It.IsAny<PaymentRequestModel>()))
                .ReturnsAsync(response);

            var controller = new PaymentsController(_mockPaymentProcessingService.Object);

            var result = await controller.Post(request);

            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((ObjectResult)result).Value.Should().Be("test");
        }

        [Test]
        public async Task Get_Should_Return_Payment_Details()
        {
            var response = new PaymentDetailsModel
                           {
                               PaymentId = Guid.NewGuid(),
                               Status = "Success",
                               Amount = 123,
                               CurrencyCode = "GBP",
                               DateTime = new DateTime(2021,9,9)
                           };

            _mockPaymentProcessingService.Setup(i => i.GetDetails(It.IsAny<Guid>()))
                .ReturnsAsync(response);

            var controller = new PaymentsController(_mockPaymentProcessingService.Object);

            var result = await controller.Get(Guid.NewGuid());

            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.OK);

            var objectResult = result as OkObjectResult;
            objectResult.Should().NotBeNull();

            var details = objectResult.Value as PaymentDetailsModel;

            details.Should().NotBeNull();
            details.Status.Should().Be(response.Status);
            details.DateTime.Should().Be(response.DateTime);
            details.Amount.Should().Be(response.Amount);
            details.CurrencyCode.Should().Be(response.CurrencyCode);
        }

        [Test]
        public async Task Get_Should_Return_NotFound()
        {
            _mockPaymentProcessingService.Setup(i => i.GetDetails(It.IsAny<Guid>()))
                .ReturnsAsync((PaymentDetailsModel)null);

            var controller = new PaymentsController(_mockPaymentProcessingService.Object);

            var result = await controller.Get(Guid.NewGuid());

            ((NotFoundResult)result).StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
