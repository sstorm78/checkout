using System;
using Checkout.Bank;
using Checkout.Bank.Models;
using Checkout.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Checkout.Services.Tests
{
    [TestFixture]
    public class PaymentProcessingServiceTests
    {
        private Mock<IPaymentsService> _mockPaymentsService;

        [SetUp]
        public void Setup()
        {
            _mockPaymentsService = new Mock<IPaymentsService>();
        }

        [Test]
        public void Process_Should_Return_Populated_PaymentResponseModel()
        {
            _mockPaymentsService.Setup(i => i.Process(It.IsAny<PaymentRequest>()))
                .Returns(new PaymentResponse().SuccessWithWarning(Guid.NewGuid(),"test"));

            var sut = new PaymentProcessingService(_mockPaymentsService.Object);

            var result = sut.Process(new PaymentRequestModel());

            result.Status.Should().Be("SuccessWithWarning");
            result.Reason.Should().Be("test");
            result.PaymentId.Should().NotBeNull();
        }

        [Test]
        public void GetDetails_Should_Return_Populated_PaymentDetailsModel()
        {
            var details = new PaymentDetails(Guid.NewGuid(), "1111222233334444", 123, DateTime.UtcNow, "Success", "GBP");

            _mockPaymentsService.Setup(i => i.GetDetails(It.IsAny<Guid>()))
                .Returns(details);

            var sut = new PaymentProcessingService(_mockPaymentsService.Object);

            var result = sut.GetDetails(Guid.NewGuid());

            result.Status.Should().Be("Success");
            result.PaymentCardNumber.Should().Be("************4444");
            result.PaymentId.Should().Be(details.PaymentId);
            result.DateTime.Should().Be(details.DateTime);
            result.CurrencyCode.Should().Be(details.CurrencyCode);
            result.Amount.Should().Be(details.Amount);
        }
    }
}
