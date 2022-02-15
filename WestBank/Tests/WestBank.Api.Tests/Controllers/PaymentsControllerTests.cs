using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using WestBank.Api.Controllers;
using WestBank.Models;
using WestBank.Services;

namespace WestBank.Api.Tests.Controllers
{
    [TestFixture]
    public class PaymentsControllerTests
    {
        private Mock<IPaymentsService> _mockPaymentService;

        [SetUp]
        public void Setup()
        {
            _mockPaymentService = new Mock<IPaymentsService>();
        }

        [Test]
        public void Post_Should_Return_Created_Result_Without_Reason()
        {
            var request = new PaymentRequest();

            var response = new PaymentResponse().Success(Guid.NewGuid());

            _mockPaymentService.Setup(i => i.Process(It.IsAny<PaymentRequest>()))
                .Returns(response);

            var controller = new PaymentsController(_mockPaymentService.Object);

            var result = controller.Post(request);

            ((CreatedResult)result).StatusCode.Should().Be((int)HttpStatusCode.Created);
            ((CreatedResult)result).Location.Should().Be($"http://localhost:60000/payments/{response.PaymentId}");
            var reason = ((CreatedResult)result).Value as string;

            reason.Should().BeNull();
        }

        [Test]
        public void Post_Should_Return_Created_Result_With_Reason()
        {
            var request = new PaymentRequest();

            var response = new PaymentResponse().SuccessWithWarning(Guid.NewGuid(), "test");

            _mockPaymentService.Setup(i => i.Process(It.IsAny<PaymentRequest>()))
                .Returns(response);

            var controller = new PaymentsController(_mockPaymentService.Object);

            var result = controller.Post(request);

            ((CreatedResult)result).StatusCode.Should().Be((int)HttpStatusCode.Created);
            ((CreatedResult)result).Location.Should().Be($"http://localhost:60000/payments/{response.PaymentId}");
            var reason = ((CreatedResult)result).Value as string;

            reason.Should().Be("test");
        }

        [Test]
        public void Post_Should_Return_BadRequest_Result_With_Reason_If_Payment_Declined()
        {
            var request = new PaymentRequest();

            var response = new PaymentResponse().Decline("test");

            _mockPaymentService.Setup(i => i.Process(It.IsAny<PaymentRequest>()))
                .Returns(response);

            var controller = new PaymentsController(_mockPaymentService.Object);

            var result = controller.Post(request);

            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((ObjectResult)result).Value.Should().Be("test");
        }

        [Test]
        public void Get_Should_Return_Payment_Details()
        {
            var response = new PaymentDetails(Guid.NewGuid(), "12333333333", 123, new DateTime(2021, 9, 9), "Success","GBP");

            _mockPaymentService.Setup(i => i.GetDetails(It.IsAny<Guid>()))
                .Returns(response);

            var controller = new PaymentsController(_mockPaymentService.Object);

            var result = controller.Get(Guid.NewGuid());

            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.OK);

            var objectResult = result as OkObjectResult;
            objectResult.Should().NotBeNull();

            var details = objectResult.Value as PaymentDetails;

            details.Should().NotBeNull();
            details.Status.Should().Be(response.Status);
            details.DateTime.Should().Be(response.DateTime);
            details.Amount.Should().Be(response.Amount);
            details.CurrencyCode.Should().Be(response.CurrencyCode);
        }

        [Test]
        public void Get_Should_Return_NotFound()
        {
            _mockPaymentService.Setup(i => i.GetDetails(It.IsAny<Guid>()))
                .Returns((PaymentDetails)null);

            var controller = new PaymentsController(_mockPaymentService.Object);

            var result = controller.Get(Guid.NewGuid());

            ((NotFoundResult)result).StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
