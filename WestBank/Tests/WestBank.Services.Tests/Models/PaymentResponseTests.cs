using System;
using FluentAssertions;
using NUnit.Framework;
using WestBank.Models;

namespace WestBank.Services.Tests.Models
{
    [TestFixture]
    public class PaymentResponseTests
    {
        [Test]
        public void Success_Should_Set_Status_To_Success()
        {
            var paymentId = Guid.NewGuid();
            var result = new PaymentResponse().Success(paymentId);

            result.Status.Should().Be(PaymentStatus.Success);
            result.Reason.Should().BeNull();
            result.PaymentId.Should().Be(paymentId);
        }

        [Test]
        public void Decline_Should_Set_Status_To_Declined_And_Populate_Reason()
        {
            var result = new PaymentResponse().Decline("test");

            result.Status.Should().Be(PaymentStatus.Declined);
            result.Reason.Should().Be("test");
            result.PaymentId.Should().BeNull();
        }

        [Test]
        public void SuccessWithWarning_Should_Set_Status_To_SuccessWithWarning_And_Populate_Reason()
        {
            var paymentId = Guid.NewGuid();
            var result = new PaymentResponse().SuccessWithWarning(paymentId, "test");

            result.Status.Should().Be(PaymentStatus.SuccessWithWarning);
            result.Reason.Should().Be("test");
            result.PaymentId.Should().Be(paymentId);
        }
    }
}
