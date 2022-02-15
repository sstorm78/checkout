namespace WestBank.Services.Rules
{
    public class RuleCheckResult
    {
        public bool Pass { get; private set; }
        public string Message { get; private set; }

        public RuleCheckResult Valid()
        {
            Pass = true;
            return this;
        }

        public RuleCheckResult Alert(string message)
        {
            Pass = false;
            Message = message;
            return this;
        }
    }
}
