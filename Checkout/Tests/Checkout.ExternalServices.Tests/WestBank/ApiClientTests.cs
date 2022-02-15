using Checkout.ExternalClients;
using Checkout.ExternalClients.WestBank;
using Checkout.ExternalClients.WestBank.Models;
using Checkout.ExternalServices.Tests.Tools;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Checkout.ExternalServices.Tests.WestBank
{
    [TestFixture]
    public class ApiClientTests
    {
        [Test]
        public async Task Pay_Should_Return_Successful_Result_With_PaymentId()
        {
            var paymentId = Guid.NewGuid();

            using (var server = new StubHttpServer())
            {

                server.SetupRoute($"/payments")
                    .Post()
                    .ReturnsStatusCode(HttpStatusCode.Created)
                    .WithHeader("location",$"http://url.com/{paymentId}")                    
                    .WithNoContent();

                var clientFactory = new RestClientFactory();

                var sut = new ApiClient(server.Url, clientFactory);

                var response = await sut.Pay(new PaymentRequest());

                response.Should().NotBeNull();
                response.Status.Should().Be(PaymentStatus.Success);
                response.PaymentId.Should().Be(paymentId);
            }
        }

        [Test]
        public async Task Pay_Should_Return_Successful_Result_With_PaymentId_And_Warning()
        {
            var paymentId = Guid.NewGuid();

            using (var server = new StubHttpServer())
            {

                server.SetupRoute($"/payments")
                    .Post()
                    .ReturnsStatusCode(HttpStatusCode.Created)
                    .WithHeader("location", $"http://url.com/{paymentId}")
                    .WithTextContent("\"Test warning\"");

                var clientFactory = new RestClientFactory();

                var sut = new ApiClient(server.Url, clientFactory);

                var response = await sut.Pay(new PaymentRequest());

                response.Should().NotBeNull();
                response.Status.Should().Be(PaymentStatus.SuccessWithWarning);
                response.Reason.Should().Be("Test warning");
                response.PaymentId.Should().Be(paymentId);
            }
        }

        [Test]
        public async Task Pay_Should_Return_Decline_Result_With_Expected_Reason()
        {
            using (var server = new StubHttpServer())
            {

                server.SetupRoute($"/payments")
                    .Post()
                    .ReturnsStatusCode(HttpStatusCode.BadRequest)
                    .WithTextContent("\"Test warning\"");

                var clientFactory = new RestClientFactory();

                var sut = new ApiClient(server.Url, clientFactory);

                Func<Task> run = () => sut.Pay(new PaymentRequest());

                await run.Should().ThrowAsync<ExternalServiceHttpException>();
            }
        }

        [Test]
        public async Task GetTransactionDetails_Should_Return_Expected_Payment_Details()
        {
            var paymentId = Guid.NewGuid();

            using (var server = new StubHttpServer())
            {
                var result = new PaymentDetails(paymentId, "", 10, DateTime.UtcNow, "ok", "GBP");

                server.SetupRoute($"/payments/{paymentId}")
                    .Get()
                    .ReturnsStatusCode(HttpStatusCode.OK)
                    .WithJsonContent(result);

                var clientFactory = new RestClientFactory();

                var sut = new ApiClient(server.Url, clientFactory);

                var response = await sut.GetTransactionDetails(paymentId);

                response.Should().NotBeNull();
                response.PaymentId.Should().Be(result.PaymentId);
            }
        }
    }
}
