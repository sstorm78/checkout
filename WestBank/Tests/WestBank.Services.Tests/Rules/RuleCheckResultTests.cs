using FluentAssertions;
using NUnit.Framework;
using WestBank.Services.Rules;

namespace WestBank.Services.Tests.Rules
{
    [TestFixture]
    public class RuleCheckResultTests
    {
        [Test]
        public void Valid_Should_Return_Pass_Equal_True_Message_Null()
        {
            var result = new RuleCheckResult().Valid();

            result.Pass.Should().BeTrue();
            result.Message.Should().BeNull();
        }

        [Test]
        public void Alert_Should_Return_Pass_Equal_False_With_Expected_Message()
        {
            var result = new RuleCheckResult().Alert("test");

            result.Pass.Should().BeFalse();
            result.Message.Should().Be("test");
        }
    }
}
