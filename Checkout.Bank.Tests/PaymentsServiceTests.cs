using System;
using System.Collections.Generic;
using Checkout.Bank.Data;
using Checkout.Bank.Data.Entities;
using Checkout.Bank.Models;
using Checkout.Bank.Validators;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Checkout.Bank.Tests
{
    [TestFixture]
    public class PaymentsServiceTests
    {
        private Mock<IDbContext> _mockDbContext;
        private Mock<IPaymentCardValidator> _mockPaymentCardValidator;
        private Mock<List<Payment>> _mockPayments;

        [SetUp]
        public void Setup()
        {
            _mockDbContext = new Mock<IDbContext>();

            _mockPayments = new Mock<List<Payment>>();

            _mockDbContext.Setup(i => i.Payments).Returns(_mockPayments.Object);

            _mockPaymentCardValidator = new Mock<IPaymentCardValidator>();
        }

        [Test]
        public void Process_Should_Return_Decline_When_Invalid_Card_Details_Provided()
        {
            var request = new PaymentRequest();

            _mockPaymentCardValidator
                .Setup(i => i.IsInfoValid(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(false);

            var sut = new PaymentsService(_mockDbContext.Object, _mockPaymentCardValidator.Object);

            var result = sut.Process(request);

            result.Status.Should().Be(PaymentStatus.Declined);
            result.Reason.Should().Be(PaymentMessages.InvalidCardDetails);
        }

        [Test]
        public void Process_Should_Return_Decline_When_Account_Has_Not_Enough_Funds()
        {
            var request = new PaymentRequest
                          {
                              PaymentCardNumber = "655555555555"
                          };

            _mockPaymentCardValidator
                .Setup(i => i.IsInfoValid(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true);

            var sut = new PaymentsService(_mockDbContext.Object, _mockPaymentCardValidator.Object);

            var result = sut.Process(request);

            result.Status.Should().Be(PaymentStatus.Declined);
            result.Reason.Should().Be(PaymentMessages.InsufficientFunds);
        }

        [Test]
        public void Process_Should_Return_Success_When_All_Details_Correct()
        {
            var request = new PaymentRequest
                          {
                              PaymentCardNumber = "2555555555555555",
                              Amount = 100,
                              CurrencyCode ="GBP",
                              CvvNumber = 123,
                              ExpiryDate = new DateTime(2021,1,1)
                          };

            _mockPaymentCardValidator
                .Setup(i => i.IsInfoValid(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true);

            var sut = new PaymentsService(_mockDbContext.Object, _mockPaymentCardValidator.Object);

            var result = sut.Process(request);

            result.Status.Should().Be(PaymentStatus.Success);
            result.Reason.Should().BeNull();
        }

        [Test]
        public void Process_Should_Return_SuccessWithWarning_When_All_Details_Correct_But_Amount_Is_High()
        {
            var request = new PaymentRequest
                          {
                              PaymentCardNumber = "2555555555555555",
                              Amount = 6000,
                              CurrencyCode = "GBP",
                              CvvNumber = 123,
                              ExpiryDate = new DateTime(2021, 1, 1)
                          };

            _mockPaymentCardValidator
                .Setup(i => i.IsInfoValid(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true);

            var sut = new PaymentsService(_mockDbContext.Object, _mockPaymentCardValidator.Object);

            var result = sut.Process(request);

            result.Status.Should().Be(PaymentStatus.SuccessWithWarning);
            result.Reason.Should().Be(PaymentMessages.UnusuallyHighAmount);
        }

        [Test]
        public void GetDetails_Should_Return_PaymentDetails()
        {
            var paymentId = Guid.NewGuid();

            var details = new Payment
            { 
                  Id = paymentId,
                  PaymentCardNumber = "2555555555555555",
                  Amount = 6000,
                  CurrencyCode = "GBP",
                  Status = "Success",
                  DateTime = new DateTime(2021, 1, 1)
            };

            var dbContext = new DbContext
            {
                Payments = new List<Payment>
                                     {
                                         details
                                     }
            };

            var sut = new PaymentsService(dbContext, _mockPaymentCardValidator.Object);

            var result = sut.GetDetails(paymentId);

            result.Should().NotBeNull();
            result.PaymentCardNumber.Should().Be("************5555");
            result.Amount.Should().Be(6000);
            result.CurrencyCode.Should().Be("GBP");
            result.Status.Should().Be("Success");
            result.DateTime.Should().Be(new DateTime(2021, 1, 1));
        }

        [Test]
        public void GetDetails_Should_Return_Null_If_Not_Found()
        {
            var paymentId = Guid.NewGuid();
            
            var dbContext = new DbContext
                            {
                                Payments = new List<Payment>()
                            };

            var sut = new PaymentsService(dbContext, _mockPaymentCardValidator.Object);

            var result = sut.GetDetails(paymentId);

            result.Should().BeNull();
        }
    }
}
