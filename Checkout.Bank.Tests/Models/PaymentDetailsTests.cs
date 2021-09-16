using System;
using Checkout.Bank.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Checkout.Bank.Tests.Models
{
    [TestFixture]
    public class PaymentDetailsTests
    {
        [Test]
        public void PaymentDetails_Constructor_Should_Populate_Properties_With_Masked_Card_Number()
        {
            var id = Guid.NewGuid();
            var cardNumber = "2555555555555555";
            var amount = 55;
            var date = new DateTime(2021, 9, 9);
            var status = "success";
            var currencyCode = "GBP";
            var result = new PaymentDetails(id, cardNumber, amount, date , status, currencyCode);

            result.PaymentId.Should().Be(id);
            result.PaymentCardNumber.Should().Be("************5555");
            result.Amount.Should().Be(amount);
            result.DateTime.Should().Be(date);
            result.Status.Should().Be(status);
            result.CurrencyCode.Should().Be(currencyCode);
        }
    }
}
