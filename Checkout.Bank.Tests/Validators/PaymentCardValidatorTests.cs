﻿using System;
using Checkout.Bank.Validators;
using FluentAssertions;
using NUnit.Framework;

namespace Checkout.Bank.Tests.Validators
{
    [TestFixture]
    public class PaymentCardValidatorTests
    {
        [TestCase("", "01/20/2020", 123, false)]
        [TestCase("0000 1298 1298 1298", "01/20/2022", 123, false)]
        [TestCase("0000 1298 1298", "01/20/2022", 123, false)]

        [TestCase("1298 1298 1298 1298", "01/20/2020", 123, false)]
        [TestCase("1298 1298 1298 1298", "01/20/2040", 123, false)]

        [TestCase("1298 1298 1298 1298", "01/20/2022", 99, false)]
        [TestCase("1298 1298 1298 1298", "01/20/2022", 100, true)]
        [TestCase("8901 1298 1298 1298", "01/20/2022", 100, true)]
        public void IsInfoValid_Should_Return_ExpectedStatus(string cardNumber, DateTime date, int cvv, bool expectedResponse)
        {
            var validator = new PaymentCardValidator();

            var response = validator.IsInfoValid(cardNumber, date, cvv);

            response.Should().Be(expectedResponse);
        }
    }
}
