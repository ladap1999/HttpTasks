using Autofac;
using TechTalk.SpecFlow;
using UserService.Clients;
using UserService.Observers;
using UserService.Steps;

namespace UserService.DI
{
     public class TestDependencyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserServiceClient>()
                .AsSelf().SingleInstance();

            builder.RegisterType<UserSteps>()
                .AsSelf();

            builder.RegisterType<UserServiceAsserts>()
                .AsSelf();

            builder.RegisterType<Transformations>()
                .AsSelf();

            builder.RegisterType<TestDataObserver>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<DeleteTestObserver>()
                .AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<ScenarioContext>()
                .AsSelf().InstancePerLifetimeScope();
        }
    }
}
