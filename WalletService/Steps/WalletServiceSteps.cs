using TechTalk.SpecFlow;
using UserService.Clients;
using UserService.Extensions;
using UserService.Steps;
using WalletService.Builders;
using WalletService.Clients;
using WalletService.Models.Request;

namespace WalletService.Steps
{
    [Binding]
    public sealed class WalletServiceSteps : UserSteps
    {
        private readonly ScenarioContext _context;

        private readonly UserServiceClient _userServiceClient;

        private WalletServiceClient _walletService;
        
        public WalletServiceSteps(ScenarioContext context, UserServiceClient userService, WalletServiceClient walletService) : base(context, userService )
        {
            _context = context;
            _userServiceClient = userService;
            _walletService = walletService;
        }

        [When(@"Created user gets balance")]
        public async Task WhenCreatedUserGetsBalance()
        {
            if (!_context.ContainsKey("responseId"))
            {
                _context["responseId"] = ((HttpResponseMessage)_context["response"]).GetId();
            }

            _context["response"] = await _walletService.GetBalance((int)_context["responseId"]);
        }

        [When(@"Get balance for user with invalid Id")]
        public async Task WhenGetBalanceForUserWithInvalidId()
        {
            var id = int.MaxValue;
            _context["response"] = await _walletService.GetBalance(id);
        }

        [When(@"User charges balance with value '(.*)'")]
        [Given(@"User charges balance with value '(.*)'")]
        public async Task WhenUserChargesBalanceWithValue(double amountOfMoney)
        {
            if (!_context.ContainsKey("responseId"))
            {
                _context["responseId"] = ((HttpResponseMessage)_context["response"]).GetId();
            }

            var charge = new ChargeBuilder(new Charge())
                .UserId((int)_context["responseId"])
                .Amount(amountOfMoney)
                .Build();
            _context["response"] = await _walletService.Charge(charge);
        }

        [When(@"Charge balance with value '(.*)' for user with invalid Id")]
        public async Task WhenChargeBalanceWithValueForUserWithInvalidId(double amountOfMoney)
        {
            var id = int.MaxValue;
            var charge = new ChargeBuilder(new Charge())
                .UserId(id)
                .Amount(amountOfMoney)
                .Build();

            _context["response"] = await _walletService.Charge(charge);
        }

        [When(@"User reverts transaction")]
        public async Task WhenUserRevertsTransaction()
        {
            _context["transactionId"] = ((HttpResponseMessage)_context["response"]).GetStringValue();
           _context["responseTransaction"] =  await _walletService.RevertTransaction(((string)_context["transactionId"]).Trim('"'));
        }

        [When(@"Make reverts transaction with icorrect id")]
        public async Task WhenMakeRevertsTransactionWithIncorrectId()
        {
            _context["transactionId"] = new Guid().ToString();
            _context["responseTransaction"] = await _walletService.RevertTransaction((string)_context["transactionId"]);
        }
    }
}