using SpecFlow.Autofac;
using Autofac;
using TechTalk.SpecFlow;
using UserService.Clients;
using UserService.Observers;

namespace UserService.DI
{
    [Binding]
    internal class SetUpFixture
    {
        private static IContainer _container;

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

            var client = _container.Resolve<UserServiceClient>();
            client.Subscribe(_container.Resolve<TestDataObserver>());
            client.Subscribe(_container.Resolve<DeleteTestObserver>());
        }

        [AfterTestRun]
        public static async Task AfterScenario()
        {
            var tasks = _container.Resolve<TestDataObserver>().GetAllIds()
                .Select(id => _container.Resolve<UserServiceClient>().DeleteUser(Convert.ToInt32(id.Value)));
            await Task.WhenAll(tasks);
        }
    }
}
