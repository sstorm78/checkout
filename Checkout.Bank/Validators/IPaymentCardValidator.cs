using System;

namespace Checkout.Bank.Validators
{
    public interface IPaymentCardValidator
    {
        bool IsInfoValid(string cardNumber, DateTime expiryDate, int cvv);
    }
}