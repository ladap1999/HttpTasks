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

        private readonly UserServiceClient _userServiceClient = UserServiceClient.Instance;
        private WalletServiceClient _walletService = new WalletServiceClient();
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks

        [BeforeScenario("@tag1")]
        public void BeforeScenarioWithTag()
        {
            // Example of filtering hooks using tags. (in this case, this 'before scenario' hook will execute if the feature/scenario contains the tag '@tag1')
            // See https://docs.specflow.org/projects/specflow/en/latest/Bindings/Hooks.html?highlight=hooks#tag-scoping

            //TODO: implement logic that has to run before executing each scenario
        }

        [BeforeScenario(Order = 1)]
        public void FirstBeforeScenario()
        {
            // Example of ordering the execution of hooks
            // See https://docs.specflow.org/projects/specflow/en/latest/Bindings/Hooks.html?highlight=order#hook-execution-order

            //TODO: implement logic that has to run before executing each scenario
        }

        [AfterScenario]
        public void AfterScenario()
        {
            //TODO: implement logic that has to run after executing each scenario
        }

        public WalletServiceSteps(ScenarioContext context) : base(context)
        {
            _context = context;
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
                .userId((int)_context["responseId"])
                .amount(amountOfMoney)
                .build();
            _context["response"] = await _walletService.Charge(charge);
        }

        [When(@"Charge balance with value '(.*)' for user with invalid Id")]
        public async Task WhenChargeBalanceWithValueForUserWithInvalidId(double amountOfMoney)
        {
            var id = int.MaxValue;
            var charge = new ChargeBuilder(new Charge())
                .userId(id)
                .amount(amountOfMoney)
                .build();

            _context["response"] = await _walletService.Charge(charge);
        }

        [When(@"User reverts transaction")]
        public async Task WhenUserRevertsTransaction()
        {
            _context["transactionId"] = ((HttpResponseMessage)_context["response"]).GetStringValue();
           _context["responseTransaction"] =  await _walletService.RevertTransaction(((string)_context["transactionId"]).Trim('"'));
        }


        [When(@"Make reverts transaction with icorrect id")]
        public async Task WhenMakeRevertsTransactionWithIcorrectId()
        {
            _context["transactionId"] = new Guid().ToString();
            _context["responseTransaction"] = await _walletService.RevertTransaction((string)_context["transactionId"]);
        }
    }
}