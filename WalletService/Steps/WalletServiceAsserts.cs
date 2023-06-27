using NUnit.Framework;
using System.Net;
using TechTalk.SpecFlow;
using UserService.Extensions;
using UserService.Steps;
using WalletService.Clients;

namespace WalletService.Steps
{
    [Binding]
    public sealed class WalletServiceAsserts : UserServiceAsserts
    {
        private readonly ScenarioContext _context;
        private WalletServiceClient _walletService = new WalletServiceClient();
        public WalletServiceAsserts(ScenarioContext context) : base(context)
        {
            _context = context;
        }

        [Then(@"Warning message is '(.*)'")]
        public void ThenWarningMessageIs(string message)
        {
            Assert.That(((HttpResponseMessage)_context["response"]).Content.ReadAsStringAsync().Result, Is.EqualTo(message));
        }

        [Then(@"Balance of user is '(.*)'")]
        public void ThenBalanceOfUserIs(double expectedBalance)
        {
            Assert.That(((HttpResponseMessage)_context["response"]).GetDoubleValue(),
                Is.EqualTo(expectedBalance));
        }

        [Then(@"Transaction status code is '(.*)'")]
        public void ThenTransactionStatusCodeIs(HttpStatusCode statusCode)
        {
            var response = (HttpResponseMessage)_context["responseTransaction"];
            Assert.That(response.StatusCode, Is.EqualTo(statusCode));
        }

        [Then(@"Transaction ends with error message")]
        public void ThenTransactionEndsWithErrorMessage()
        {
            Assert.That(((HttpResponseMessage)_context["responseTransaction"]).GetStringValue(),
                Is.EqualTo($"Transaction '{((string)_context["transactionId"]).Trim('"')}'" +
                           $" cannot be reversed due to 'Reverted' current status"));
        }

        [Then(@"Transaction ends with error message '(.*)'")]
        public void ThenTransactionEndsWithErrorMessage(string message)
        {
            Assert.That(((HttpResponseMessage)_context["responseTransaction"]).GetStringValue(),
                Is.EqualTo(message));
        }
    }

}