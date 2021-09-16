using System;
using System.Text.RegularExpressions;

namespace Checkout.Bank.Validators
{
    public class PaymentCardValidator : IPaymentCardValidator
    {
        public bool IsInfoValid(string cardNumber, DateTime expiryDate, int cvv)
        {
            var cardCheck = new Regex(@"^(1298|1267|4512|4567|8901|8933)([\-\s]?[0-9]{4}){3}$");
            var monthCheck = new Regex(@"^(0[1-9]|1[0-2])$");
            var yearCheck = new Regex(@"^20[0-9]{2}$");

            if (!cardCheck.IsMatch(cardNumber)) // <1>check card number is valid
                return false;
            if (cvv < 100 || cvv > 1000) // <2>check cvv is valid by being between 100 and 999
                return false;
            
            var lastDateOfExpiryMonth = DateTime.DaysInMonth(expiryDate.Year, expiryDate.Month); //get actual expiry date
            var cardExpiry = new DateTime(expiryDate.Year, expiryDate.Month, lastDateOfExpiryMonth, 23, 59, 59);

            //check expiry greater than today & within next 6 years <7, 8>>
            return (cardExpiry > DateTime.Now && cardExpiry < DateTime.Now.AddYears(6));
        }
    }
}
