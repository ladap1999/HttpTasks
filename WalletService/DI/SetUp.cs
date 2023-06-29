using SpecFlow.Autofac;
using Autofac;
using TechTalk.SpecFlow;
using UserService.Clients;
using UserService.Observers;
using WalletService.observers;
using System;
using WalletService.Clients;


namespace WalletService.DI
{
    [Binding]
    internal class SetUp
    {
        private static IContainer _container;
        private static TestDataObserver _testDataObserver;
        private static TransactionTestObserver _transactionTestObserver;

        [ScenarioDependencies]
        public static ContainerBuilder ScenarioDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<TestDependencyModule>();
            return builder;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _container = ScenarioDependencies().Build();

            /*_observerForTransaction = new TransactionTestObserver();
            _observer = new TestDataObserver();
            WalletServiceClient.Instance.Subscribe(_observerForTransaction);
            UserServiceClient.Instance.Subscribe(_observer);*/

            var userClient = _container.Resolve<UserServiceClient>();
            var walletClient = _container.Resolve<WalletServiceClient>();

            walletClient.Subscribe(_container.Resolve<TransactionTestObserver>());
            userClient.Subscribe(_container.Resolve<TestDataObserver>());
        }

        [AfterTestRun]
        public static async Task AfterScenario()
        {
            var tasks = _container.Resolve<TransactionTestObserver>().GetAllIds()
                .Select(id => _container.Resolve<UserServiceClient>().DeleteUser(Convert.ToInt32(id.Value)));

            await Task.WhenAll(tasks);
        }
    }
}
