using WestBank.Models;
using WestBank.Services.Models;

namespace WestBank.Services.Rules
{
    public class AbnormalAmount: IRule
    {
        public RuleCheckResult Check(PaymentRequest request)
        {
            if(request.Amount > 5000)
            {
                return new RuleCheckResult().Alert(PaymentMessages.UnusuallyHighAmount);
            }

            return new RuleCheckResult().Valid();
        }
    }
}
