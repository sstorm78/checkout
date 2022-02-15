using System;
using System.Collections.Generic;
using System.Linq;
using WestBank.Data;
using WestBank.Data.Entities;
using WestBank.Models;
using WestBank.Services.Models;
using WestBank.Services.Rules;
using WestBank.Services.Validators;

namespace WestBank.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IDbContext _dbContext;
        private readonly IPaymentCardValidator _paymentCardValidator;
        private readonly IEnumerable<IRule> _rules;

        public PaymentsService(
            IDbContext dbContext,
            IPaymentCardValidator paymentCardValidator,
            IEnumerable<IRule> rules)
        {
            _dbContext = dbContext;
            _paymentCardValidator = paymentCardValidator;
            _rules = rules;
        }

        public PaymentResponse Process(PaymentRequest paymentRequest)
        {
            if (!_paymentCardValidator.IsInfoValid(paymentRequest.PaymentCardNumber, paymentRequest.ExpiryDate, paymentRequest.CvvNumber))
            {
                return new PaymentResponse().Decline(PaymentMessages.InvalidCardDetails);
            }

            // for the test purposes, any payment card with the first digit greater than 5
            // will return insuficient funds response
            if (int.Parse(paymentRequest.PaymentCardNumber.Substring(0, 1)) > 5)
                return new PaymentResponse().Decline(PaymentMessages.InsufficientFunds);

            var alerts = CheckAgainstRules(paymentRequest);

            var status = string.IsNullOrEmpty(alerts) ? PaymentStatus.Success: PaymentStatus.SuccessWithWarning;

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
                new PaymentResponse().SuccessWithWarning(payment.Id, alerts) 
                : new PaymentResponse().Success(payment.Id);
        }

        private string CheckAgainstRules(PaymentRequest request)
        {
            var alerts = new List<string>();

            foreach(var rule in _rules)
            {
                var outcome = rule.Check(request);
                if (!outcome.Pass)
                {
                    alerts.Add(outcome.Message);
                }
            }

            return string.Join(',', alerts);
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
