using System;

namespace WestBank.Services.Validators
{
    public interface IPaymentCardValidator
    {
        bool IsInfoValid(string cardNumber, DateTime expiryDate, int cvv);
    }
}