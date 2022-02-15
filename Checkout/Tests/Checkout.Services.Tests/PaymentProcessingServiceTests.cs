using System;
using System.Threading.Tasks;
using Checkout.ExternalClients.WestBank;
using Checkout.ExternalClients.WestBank.Models;
using Checkout.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Checkout.Services.Tests
{
    [TestFixture]
    public class PaymentProcessingServiceTests
    {
        private Mock<IApiClient> _apiClient;

        [SetUp]
        public void Setup()
        {
            _apiClient = new Mock<IApiClient>();
        }

        [Test]
        public async Task Process_Should_Return_Populated_PaymentResponseModel()
        {
            _apiClient.Setup(i => i.Pay(It.IsAny<PaymentRequest>()))
                .ReturnsAsync(new PaymentResponse().SuccessWithWarning(Guid.NewGuid(),"test"));

            var sut = new PaymentProcessingService(_apiClient.Object);

            var result = await sut.Process(new PaymentRequestModel());

            result.Status.Should().Be("SuccessWithWarning");
            result.Reason.Should().Be("test");
            result.PaymentId.Should().NotBeNull();
        }

        [Test]
        public async Task GetDetails_Should_Return_Populated_PaymentDetailsModel()
        {
            var details = new PaymentDetails(Guid.NewGuid(), "1111222233334444", 123, DateTime.UtcNow, "Success", "GBP");

            _apiClient.Setup(i => i.GetTransactionDetails(It.IsAny<Guid>()))
                .ReturnsAsync(details);

            var sut = new PaymentProcessingService(_apiClient.Object);

            var result = await sut.GetDetails(Guid.NewGuid());

            result.Status.Should().Be("Success");
            result.PaymentCardNumber.Should().Be("************4444");
            result.PaymentId.Should().Be(details.PaymentId);
            result.DateTime.Should().Be(details.DateTime);
            result.CurrencyCode.Should().Be(details.CurrencyCode);
            result.Amount.Should().Be(details.Amount);
        }
    }
}
