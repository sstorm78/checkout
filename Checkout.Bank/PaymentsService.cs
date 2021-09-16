using System;
using System.Linq;
using Checkout.Bank.Data;
using Checkout.Bank.Data.Entities;
using Checkout.Bank.Models;
using Checkout.Bank.Validators;

namespace Checkout.Bank
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IDbContext _dbContext;
        private readonly IPaymentCardValidator _paymentCardValidator;

        public PaymentsService(
            IDbContext dbContext,
            IPaymentCardValidator paymentCardValidator)
        {
            _dbContext = dbContext;
            _paymentCardValidator = paymentCardValidator;
        }

        public PaymentResponse Process(PaymentRequest paymentRequest)
        {
            if (!_paymentCardValidator.IsInfoValid(paymentRequest.PaymentCardNumber, paymentRequest.ExpiryDate, paymentRequest.CvvNumber))
            {
                return new PaymentResponse().Decline(PaymentMessages.InvalidCardDetails);
            }

            if (int.Parse(paymentRequest.PaymentCardNumber.Substring(0, 1)) > 5)
                return new PaymentResponse().Decline(PaymentMessages.InsufficientFunds);

            var status = paymentRequest.Amount > 5000 ? PaymentStatus.SuccessWithWarning : PaymentStatus.Success;

            var payment = new Payment
                              {
                                  Id = Guid.NewGuid(),
                                  PaymentCardNumber = paymentRequest.PaymentCardNumber,
                                  Amount = paymentRequest.Amount,
                                  DateTime = DateTime.UtcNow,
                                  CurrencyCode = paymentRequest.CurrencyCode,
                                  Status = Enum.GetName(typeof(PaymentStatus), status)
                              };

            _dbContext.Payments.Add(payment);
            //SaveChanges();

            return status == PaymentStatus.SuccessWithWarning ? 
                new PaymentResponse().SuccessWithWarning(payment.Id, PaymentMessages.UnusuallyHighAmount) 
                : new PaymentResponse().Success(payment.Id);
        }

        public PaymentDetails GetDetails(Guid paymentId)
        {
            return _dbContext
                .Payments
                .Where(i => i.Id == paymentId)
                .Select(i => new PaymentDetails(i.Id, i.PaymentCardNumber, i.Amount, i.DateTime, i.Status, i.CurrencyCode))
                .FirstOrDefault();
        }
    }
}
