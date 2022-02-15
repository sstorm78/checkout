using WestBank.Models;

namespace WestBank.Services.Rules
{
    public interface IRule
    {
        RuleCheckResult Check(PaymentRequest request);
    }
}
